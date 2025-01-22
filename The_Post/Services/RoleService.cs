
using Microsoft.AspNetCore.Identity;

namespace The_Post.Services
{
    public class RoleService : IRoleService
    {
        public async Task AddRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteRole(string roleID)
        {
            throw new NotImplementedException();
        }

        public async Task EditRole(string roleID)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityRole> GetAllRoles() // I'm not sure if this is the correct method signature
        {
            throw new NotImplementedException();
        }
    }
}
