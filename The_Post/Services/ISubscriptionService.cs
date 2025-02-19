using The_Post.Models;

namespace The_Post.Services
{
    public interface ISubscriptionService
    {
        Task<bool> AddSubscription(string userId, int subscriptionTypeId);
        Task<bool> RenewSubscription(string userId, int months = 1);
        Task<bool> CancelSubscriptionAsync(string userId);
    }
}
