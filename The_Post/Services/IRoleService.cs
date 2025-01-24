using Microsoft.AspNetCore.Identity;

namespace The_Post.Services
{
    public interface IRoleService
    {
        public Task AddRole(string roleName);        
        public Task EditRole(IdentityRole role);
        public Task DeleteRole(string roleId);
        public Task<List<IdentityRole>> GetAllRoles();
    }
}
