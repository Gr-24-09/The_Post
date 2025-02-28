using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using The_Post.Data;
using The_Post.Models;
using The_Post.Models.VM;

namespace The_Post.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public SubscriptionService(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<Subscription?> AddSubscription(string userId, int subscriptionTypeId)
        {
            // Retrieve the subscription type details
            var subscriptionType = await _db.SubscriptionTypes.FindAsync(subscriptionTypeId);
            if (subscriptionType == null)
            {
                return null;
            }

            // Create a new subscription record
            var subscription = new Subscription
            {
                SubscriptionTypeId = subscriptionTypeId,                
                HistoricalPrice = subscriptionType.Price,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMonths(1),
                PaymentComplete = true, // Since payment was confirmed via success handler
                UserId = userId
            };

            _db.Subscriptions.Add(subscription);
            await _db.SaveChangesAsync();
                       
            // Assign the "Subscriber" role to the user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            // Add the Subscriber role while keeping the Member role intact.
            var addRoleResult = await _userManager.AddToRoleAsync(user, "Subscriber");
            if (!addRoleResult.Succeeded)
            {
                return null;
            }

            return subscription;
        }

        
        // Method to cancel the subscription
        public async Task<bool> CancelSubscriptionAsync(string userId)
        {
            var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.UserId == userId && s.Expires > DateTime.UtcNow);

            if (subscription == null)
            {
                return false;
            }                

            subscription.Expires = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _userManager.IsInRoleAsync(user, "Subscriber"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Subscriber");
            }

            return true;
        }

        public async Task<SubscriptionStatsVM> GetSubscriptionStats()
        {
            var now = DateTime.UtcNow;
            var totalSubscribers = await _db.Subscriptions.CountAsync();
            var activeSubscriptions = await _db.Subscriptions.CountAsync(s => s.Expires > now);
            var expiredSubscriptions = totalSubscribers - activeSubscriptions;

            return new SubscriptionStatsVM
            {
                TotalSubscribers = totalSubscribers,
                ActiveSubscriptions = activeSubscriptions,
                ExpiredSubscriptions = expiredSubscriptions
            };
        }

        public async Task<List<SubscriptionStatsVM>> GetSubscriptionStatsOverTime()
        {
            // Get all subscriptions
            var subscriptions = await _db.Subscriptions.ToListAsync();
            var now = DateTime.UtcNow;

            // Group in memory
            var stats = subscriptions
                .GroupBy(s => new { Year = s.Created.Year, Month = s.Created.Month })
                .Select(g => new SubscriptionStatsVM
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1), // First day of the month
                    TotalSubscribers = g.Count(),
                    ActiveSubscriptions = g.Count(s => s.Expires > now),
                    ExpiredSubscriptions = g.Count(s => s.Expires <= now)
                })
                .OrderBy(s => s.Month)
                .ToList();

            // If no data, provide at least one month of data (current month)
            if (!stats.Any())
            {
                stats.Add(new SubscriptionStatsVM
                {
                    Month = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                    TotalSubscribers = 0,
                    ActiveSubscriptions = 0,
                    ExpiredSubscriptions = 0
                });
            }

            return stats;
        }
    }    
}
