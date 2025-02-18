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

        public async Task<bool> AddSubscription(string userId, int subscriptionTypeId)
        {
            // Retrieve the subscription type details
            var subscriptionType = await _db.SubscriptionTypes.FindAsync(subscriptionTypeId);
            if (subscriptionType == null)
            {
                return false;
            }

            // Create a new subscription record
            var subscription = new Subscription
            {
                SubscriptionTypeId = subscriptionTypeId,                
                HistoricalPrice = subscriptionType.Price,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddYears(1),
                PaymentComplete = false, // Initially false until payment is confirmed
                UserId = userId
            };

            _db.Subscriptions.Add(subscription);
            await _db.SaveChangesAsync();

            // ---- Payment processing logic here ----
            // Need to implement Stripe

            // For simplicity, for now assume the payment goes through successfully:
            bool paymentSuccess = true;

            if (!paymentSuccess)
            {
                // Delete the subscription record if payment fails.
                _db.Subscriptions.Remove(subscription);
                await _db.SaveChangesAsync();
                return false;
            }

            // Update the subscription to mark payment complete
            subscription.PaymentComplete = true;
            await _db.SaveChangesAsync();

            // Assign the "Subscriber" role to the user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Add the Subscriber role while keeping the Member role intact.
            var addRoleResult = await _userManager.AddToRoleAsync(user, "Subscriber");
            return addRoleResult.Succeeded;
        }
    }
    
}
