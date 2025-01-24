using Microsoft.AspNetCore.Identity;
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

        public async Task AddEmployee(User user)
        {
           await _userManager.CreateAsync(user);                      
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

        public async Task EditEmployee(string userId, User updatedUser)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }

                user.UserName = updatedUser.UserName;
                user.Email = updatedUser.Email;
                user.PhoneNumber = updatedUser.PhoneNumber;
                user.DOB = updatedUser.DOB;
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;

                await _userManager.UpdateAsync(user);           
        }

        public List<User> GetAllEmployees()
        {
            return _userManager.Users.ToList();
        }
    }
}
