using The_Post.Models;

namespace The_Post.Services
{
    public interface ISubscriptionService
    {
        Task<Subscription?> AddSubscription(string userId, int subscriptionTypeId);        
        Task<bool> CancelSubscriptionAsync(string userId);
    }
}
