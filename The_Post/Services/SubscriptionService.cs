using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using The_Post.Data;
using The_Post.Models;

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
    }    
}
