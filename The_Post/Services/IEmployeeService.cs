﻿using The_Post.Models;

namespace The_Post.Services
{
    public interface IEmployeeService
    {
        public Task AddEmployee(User employee);
        public Task AssignRole(string userId, string role);        
        public Task DeleteEmployee(string userId);
        public Task EditEmployee(User updatedUser);
        public Task<List<User>> GetAllEmployees();
    }
}
