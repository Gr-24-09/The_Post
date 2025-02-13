﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using The_Post.Data;
using The_Post.Models;
using The_Post.Models.VM;
using The_Post.Services;
using The_Post.Middleware;

namespace The_Post.Controllers
{
    public class AdminController : Controller
    {        
        private readonly IArticleService _articleService;
        private readonly IEmployeeService _employeeService;
        private readonly IRoleService _roleService;
        private readonly ApplicationDbContext _db;
        private const int MaxEditorsChoice = 5;

        public AdminController(IArticleService articleService, IEmployeeService employeeService, IRoleService roleService, ApplicationDbContext db)
        {            
            _articleService = articleService;
            _employeeService = employeeService;
            _roleService = roleService;
            _db = db;
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
            var employees = await _employeeService.GetAllEmployeesWithRolesAsync();
            return View(employees);
        }

        //------------------------- OTHER ACTIONS -------------------------

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
                
        public IActionResult SubscriptionStats()
        {
            return View();
        }
    }
}
