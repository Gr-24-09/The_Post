using The_Post.Models;

namespace The_Post.Services
{
    public interface ISubscriptionService
    {
        Task<Subscription?> AddSubscription(string userId, int subscriptionTypeId);
        Task SendConfirmationEmailAsync(string userId, int subscriptionTypeId, string baseUrl);
        
        Task<bool> CancelSubscriptionAsync(string userId);
    }
}
