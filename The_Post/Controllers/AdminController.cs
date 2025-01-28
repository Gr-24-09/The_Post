using Microsoft.AspNetCore.Mvc;
using The_Post.Data;
using The_Post.Models;
using The_Post.Models.VM;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class AdminController : Controller
    {        
        private readonly IArticleService _articleService;
        private readonly IEmployeeService _employeeService;
        private readonly IRoleService _roleService;

        public AdminController(IArticleService articleService, IEmployeeService employeeService, IRoleService roleService)
        {            
            _articleService = articleService;
            _employeeService = employeeService;
            _roleService = roleService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        //------------------------- EMPLOYEE ACTIONS -------------------------

        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {                    
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DOB = model.DOB,
                    Address = model.Address,
                    Zip = model.Zip,
                    City = model.City,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    IsEmployee = true
                };

                var result = await _employeeService.AddEmployee(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("EmployeeAdded");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                } 
            }

            return View(model);
        }

        public IActionResult EmployeeAdded()
        {
            return View();
        }

        public IActionResult EditEmployee()
        {
            return View();
        }

        public async Task<IActionResult> DeleteEmployee(string userId)
        {
            return View();
        }

        public async Task<IActionResult> AllEmployees()
        {
            return View(await _employeeService.GetAllEmployees());
        }

        //------------------------- OTHER ACTIONS -------------------------

        public IActionResult EditorsChoice()
        {
            return View();
        }

        public IActionResult ArchiveArticle()
        {
            return View();
        }

        public IActionResult AssignRole()
        {
            return View();
        }

        public IActionResult SubscriptionStats()
        {
            return View();
        }
    }
}
