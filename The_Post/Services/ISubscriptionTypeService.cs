using The_Post.Models;

namespace The_Post.Services
{
    public interface ISubscriptionTypeService
    {
        Task<List<SubscriptionType>> GetAllSubTypes();
        Task<SubscriptionType?> GetByIdAsync(int id);
        Task CreateSubType(SubscriptionType subType);
        Task EditSubType(SubscriptionType subType);
        Task DeleteSubType(int id);
        Task<bool> Exists(int id);
    }
}