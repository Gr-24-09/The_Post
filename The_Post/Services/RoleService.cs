
using Microsoft.AspNetCore.Identity;

namespace The_Post.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task AddRole(string roleName)
        {
          if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole (roleName));    
            }
        }

        public async Task DeleteRole(string roleID)
        {
            var Role = await _roleManager.FindByIdAsync (roleID);
            if (Role != null)
            {
                await _roleManager.DeleteAsync (Role);
            }
        }

        public async Task EditRole(string roleID, string NewRoleName)
        {
            var existingRole = await _roleManager.FindByIdAsync(roleID);
            if (existingRole != null)
            {
                existingRole.Name = NewRoleName;
                await _roleManager.UpdateAsync (existingRole);
            }
        }

        public async Task<List<IdentityRole>> GetAllRoles() 
        {
            return _roleManager.Roles.ToList();
        }
    }
}
