using Microsoft.AspNetCore.Mvc;
using The_Post.Data;
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

        public IActionResult Indx()
        {
            return View();
        }

        public IActionResult AddArticle()
        {
            return View();
        }

        public IActionResult EditArticle()
        {
            return View();
        }

        public IActionResult DeleteArticle()
        {
            return View();
        }

        public IActionResult ArticleAddedSuccessfully()
        {
            return View();
        }

        public IActionResult AllArticles()
        {
            return View();
        }

        public IActionResult EditorsChoice()
        {
            return View();
        }

        public IActionResult SubscriptionStats()
        {
            return View();
        }

        public IActionResult AddEmployee()
        {
            return View();
        }

        public IActionResult EditEmployee()
        {
            return View();
        }

        public IActionResult DeleteEmployee()
        {
            return View();
        }

        public IActionResult AllEmployees()
        {
            return View();
        }

        public IActionResult AssignRole()
        {
            return View();
        }

        public IActionResult ArchiveArticle()
        {
            return View();
        }
    }
}
