using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using The_Post.Models;
using The_Post.Services;

namespace The_Post.Areas.Identity.Pages.Account.Manage
{
    public class NewslettersModel : PageModel
    {
        private readonly IArticleService _articleService;

        public List<SelectListItem> AvailableCategories { get; set; }

        [BindProperty]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public NewslettersModel(IArticleService articleService)
        {
            _articleService = articleService;
        }
        public void OnGet()
        {
            AvailableCategories = _articleService.GetAllCategoriesSelectList();
        }

        public void OnPostSave()
        {
            if (!ModelState.IsValid)
            {
                return;
            }
        }
    }
}
