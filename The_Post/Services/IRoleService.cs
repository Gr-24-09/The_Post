using Microsoft.AspNetCore.Identity;

namespace The_Post.Services
{
    public interface IRoleService
    {
        public Task AddRole(string roleName);
        public Task<IdentityRole> GetAllRoles(); // I'm not sure if this is the correct signature
        public Task EditRole(string roleID);
        public Task DeleteRole(string roleID);
    }
}
