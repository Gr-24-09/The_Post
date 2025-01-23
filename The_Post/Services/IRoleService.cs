using Microsoft.AspNetCore.Identity;

namespace The_Post.Services
{
    public interface IRoleService
    {
        public Task AddRole(string roleName);
        public Task<List<IdentityRole>> GetAllRoles(); // I'm not sure if this is the correct return type, maybe it should be a list of string names?
        public Task EditRole(string roleID , string NewRoleName);
        public Task DeleteRole(string roleID);
    }
}
