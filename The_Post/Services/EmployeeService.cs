using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;
using The_Post.Data;
using The_Post.Models;

namespace The_Post.Services
{
    public class EmployeeService : IEmployeeService
    {       
        private readonly UserManager<User> _userManager;

        public EmployeeService(UserManager<User> userManager)
        {            
            _userManager = userManager;
        }

        public async Task AddEmployee(User user, string password)
        {
           await _userManager.CreateAsync(user, password);                      
        }

        public async Task AssignRole(string userId, string role)
        {            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
            { 
                throw new ArgumentException("User not found", nameof(userId));
            }
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task DeleteEmployee(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }
            await _userManager.DeleteAsync(user);            
        }

        public async Task EditEmployee(User user)
        {                         
            await _userManager.UpdateAsync(user);           
        }

        public async Task<List<User>> GetAllEmployees()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}
