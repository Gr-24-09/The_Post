
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

        public async Task<List<IdentityRole>> GetAllRoles() // I'm not sure if this is the correct return type, maybe it should be a list of string names?
        {
            throw new NotImplementedException();
        }
    }
}
