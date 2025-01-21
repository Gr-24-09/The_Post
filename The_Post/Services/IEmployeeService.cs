using The_Post.Models;

namespace The_Post.Services
{
    public interface IEmployeeService
    {
        public Task AddEmployee(User employee);
        public Task<User> GetAllEmployees();
        public Task EditEmployee(string userId);
        public Task DeleteEmployee(string userId);
        public Task AssignRole(string userId); // Add Role as a parameter
    }
}
