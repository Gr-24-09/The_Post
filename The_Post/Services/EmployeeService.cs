﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;
using The_Post.Data;
using The_Post.Models;
using The_Post.Models.VM;

namespace The_Post.Services
{
    public class EmployeeService : IEmployeeService
    {       
        private readonly UserManager<User> _userManager;

        public EmployeeService(UserManager<User> userManager)
        {            
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddEmployee(User user, string password)
        {
           return await _userManager.CreateAsync(user, password);                      
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

        public async Task<bool> DeleteEmployee(string userId)
        {
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {   
                await _userManager.RemoveFromRolesAsync(user, roles);
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error deleting user: {error.Description}"); // ✅ Log error details
                }
            }
            return result.Succeeded;
        }

        public async Task EditEmployee(User user)
        {                         
            await _userManager.UpdateAsync(user);           
        }

        public async Task<List<User>> GetAllEmployees()
        {
            var employees = await _userManager.Users
                .Where(u => u.IsEmployee)
                .ToListAsync();

            return employees;
        }

        public async Task<List<EmployeeVM>> GetAllEmployeesWithRolesAsync()
        {
            var employees = await _userManager.Users
                .Where(u => u.IsEmployee)
                .ToListAsync();

            var employeeVMs = new List<EmployeeVM>();

            foreach (var user in employees)
            {
                var roles = await _userManager.GetRolesAsync(user);
                employeeVMs.Add(new EmployeeVM
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    UserName = user.UserName,
                    DOB = user.DOB,
                    Address = user.Address,
                    Zip = user.Zip,
                    City = user.City,
                    Role = roles.Any() ? string.Join(", ", roles) : "No Role Assigned"
                });
            }

            return employeeVMs;
        }
    }
}
