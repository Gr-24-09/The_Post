using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using The_Post.Models;
using The_Post.Services;

namespace The_Post.Areas.Identity.Pages.Account.Manage
{
    public class NewslettersModel : PageModel
    {
        private readonly IArticleService _articleService;
        private readonly UserManager<User> _userManager;

        public List<SelectListItem> AvailableCategories { get; set; }

        [BindProperty] // Binds the property to the request data so that the property is automatically populated with the request data
        public bool EditorsChoiceSelection { get; set; }

        [BindProperty]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public NewslettersModel(IArticleService articleService, UserManager<User> userManager)
        {
            _articleService = articleService;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var loggedInUser = await _userManager.Users
                .Where(u => u.Id == _userManager.GetUserId(User))
                .Include(u => u.NewsletterCategories) // Laddar kategorierna explicit
                .FirstOrDefaultAsync();

            AvailableCategories = _articleService.GetAllCategoriesSelectList();
            EditorsChoiceSelection = loggedInUser.EditorsChoiceNewsletter;
            SelectedCategoryIds = loggedInUser.NewsletterCategories.Select(c => c.Id).ToList();
        }

    
public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var loggedInUser = await _userManager.Users
                .Where(u => u.Id == _userManager.GetUserId(User))
                .Include(u => u.NewsletterCategories) // Needed for adding/removing categories
                .FirstOrDefaultAsync();

            // Remove all existing category relationships (Ensures DB changes are tracked)
            loggedInUser.NewsletterCategories.Clear();

            // Get the selected categories
            var selectedCategories = _articleService.GetSelectedCategories(SelectedCategoryIds);

            // Add new selected categories
            // EF Core will automatically track changes to the collection, but only if added one by one
            foreach (var category in selectedCategories)
            {
                loggedInUser.NewsletterCategories.Add(category);
            }

            // Update the Editors' Choice selection
            loggedInUser.EditorsChoiceNewsletter = EditorsChoiceSelection;

            // Save changes
            var result = await _userManager.UpdateAsync(loggedInUser);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your Newsletter preferences have been saved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to save your preferences. Please try again.";
            }
      

            return RedirectToPage();
        }

    
    }
}
