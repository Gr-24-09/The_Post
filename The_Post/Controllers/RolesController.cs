using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();  
            return View(roles);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["ErrorMessage"] = "Role name cannot be empty .";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                await _roleService.AddRoleAsync(roleName);
                TempData["SuccessMessage"] = "Role added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit (string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Role Not Found.";
                return RedirectToAction(nameof(Index));
            }
           return View(role);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(IdentityRole role)
        {
            if (role == null || string.IsNullOrEmpty(role.Id))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(role);
            }
           
            var existingRole = await _roleService.GetRoleByIdAsync(role.Id);
            if (existingRole == null)
            {
                TempData["ErrorMessage"] = "Role not found.";
                return RedirectToAction(nameof(Index));
            }

            existingRole.Name = role.Name;
            await _roleService.EditRoleAsync(existingRole);

            TempData["SuccessMessage"] = "Role updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                await _roleService.DeleteRoleAsync(id);
                TempData["SuccessMessage"] = "Role deleted successfully.";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
