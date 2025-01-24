
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using The_Post.Models;

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

        public async Task DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync (roleId);
            if (role == null)
            {
                throw new ArgumentException("Role not found", nameof(roleId));
            }
            await _roleManager.DeleteAsync(role);
        }

        public async Task EditRole(IdentityRole role)
        {           
            await _roleManager.UpdateAsync(role);            
        }

        public async Task<List<IdentityRole>> GetAllRoles() 
        {
            return await _roleManager.Roles.ToListAsync();
        }
    }
}
