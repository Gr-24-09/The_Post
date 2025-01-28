using Microsoft.AspNetCore.Identity;
using The_Post.Models;

namespace The_Post.Services
{
    public interface IEmployeeService
    {
        public Task<IdentityResult> AddEmployee(User user, string password);
        public Task AssignRole(string userId, string role);        
        public Task DeleteEmployee(string userId);
        public Task EditEmployee(User user);
        public Task<List<User>> GetAllEmployees();
    }
}
