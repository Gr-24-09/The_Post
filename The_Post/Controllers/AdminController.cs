using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using The_Post.Data;
using The_Post.Models;
using The_Post.Models.VM;
using The_Post.Services;
using The_Post.Middleware;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using Stripe;

namespace The_Post.Controllers
{
    
    public class AdminController : Controller
    {        
        private readonly IArticleService _articleService;
        private readonly IEmployeeService _employeeService;
        private readonly IRoleService _roleService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ApplicationDbContext _db;
        private const int MaxEditorsChoice = 5;

        public AdminController(IArticleService articleService, IEmployeeService employeeService, IRoleService roleService, ISubscriptionService subscriptionService,ApplicationDbContext db)
        {            
            _articleService = articleService;
            _employeeService = employeeService;
            _roleService = roleService;
            _subscriptionService = subscriptionService;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        //------------------------- EMPLOYEE ACTIONS -------------------------



        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditEmployee(string userId)
        {
            var employee = await _employeeService.GetEmployeeById(userId);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(EditEmployeeVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _employeeService.EditEmployee(model);
            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to update employee details.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Employee details updated successfully.";
            return RedirectToAction("AllEmployees");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
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


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(string userId)
        {
            var result = await _employeeService.DeleteEmployee(userId);

            if (!result) 
            {
                TempData["ErrorMessage"] = "Failed to delete employee.";
                return RedirectToAction("AllEmployees"); 
            }

            TempData["SuccessMessage"] = "Employee deleted successfully.";
            return RedirectToAction("AllEmployees");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesWithRolesAsync();
            return View(employees);
        }

        //------------------------- OTHER ACTIONS -------------------------

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole()
        {
            var model = new AssignRoleVM
            {
                Users = await _employeeService.GetAllEmployees(),
                Roles = await _roleService.GetAllRolesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(AssignRoleVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.AssignRole(model.UserId, model.Role);
                    return RedirectToAction("AllEmployees");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                } 
            }

            //Repopulate dropdowns if validation fails
            model.Users = await _employeeService.GetAllEmployees();
            model.Roles = await _roleService.GetAllRolesAsync();
            return View(model);
        }

        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> UpdateEditorsChoice(int articleId, bool isEditorsChoice)
        {
            try
            {
                var article = await _db.Articles.FindAsync(articleId);
                if (article == null)
                {
                    return NotFound("Article not found");
                }

                if (isEditorsChoice)
                {
                    var currentCount = await _db.Articles
                        .Where(a => a.EditorsChoice)
                        .CountAsync();

                    if(currentCount >= MaxEditorsChoice)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Maximum number of Editor's Choices ({MaxEditorsChoice}) has been reached."
                        });
                    }
                }

                article.EditorsChoice = isEditorsChoice;
                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> ArchiveArticle([FromBody] ArchiveArticleRequest request)
        {
            try
            {
                var article = await _db.Articles.FindAsync(request.ArticleId);
                if (article == null)
                {
                    return NotFound(new { success = false, message = "Article not found" });
                }
                article.IsArchived = request.IsArchived;
                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred" });
            }
        }


        [Authorize(Roles = "Admin,Editor")]
        // Gets the search results in a paginated list
        public IActionResult SearchResultsAdmin(string searchTerm, int? page)
        {
            // Sets the page number to 1 if the page-parameter is null
            int pageNumber = page ?? 1;
            int pageSize = 12;

            var articles = _articleService.GetSearchResults(searchTerm);
            var onePageOfArticles = articles.ToPagedList(pageNumber, pageSize);
            
            SearchVM searchVM = new SearchVM
            {
                Articles = onePageOfArticles,
                SearchTerm = searchTerm
            };

            return View(searchVM);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SubscriptionStats()
        {
            var stats = await _subscriptionService.GetSubscriptionStats();
            return View(stats); 
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SubscriptionStatsOverTime()
        {
            var stats = await _subscriptionService.GetSubscriptionStatsOverTime();
            var jsonData = stats.Select(s => new
            {
                Month = s.Month.ToString("yyyy-MM"), // Format for the X-axis
                TotalSubscribers = s.TotalSubscribers,
                ActiveSubscriptions = s.ActiveSubscriptions,
                ExpiredSubscriptions = s.ExpiredSubscriptions
            }).ToList();

            return Json(jsonData);
        }
    }
}


/*
basel @example.com
B_e123456

john.editor@email.com
John@1234

emily.admin@email.com
Emily@5678

ahmed.writer@email.com
Ahmed@9876
*/