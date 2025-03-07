using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class BaseCookiesController : Controller
    {
        private readonly IArticleService _articleService;

        public BaseCookiesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Set ViewBag.IsCookiesAccepted for all actions in controllers that inherit from this controller
            ViewBag.IsCookiesAccepted = _articleService.IsCookiesAccepted();
            base.OnActionExecuted(context);
        }

        [HttpPost]
        public IActionResult AcceptCookies()
        {
            // Set the cookie to mark the user has accepted cookies
            _articleService.AcceptCookies();

            return NoContent();
        }
    }
}
