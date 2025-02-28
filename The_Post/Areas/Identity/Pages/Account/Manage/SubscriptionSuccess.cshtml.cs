using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace The_Post.Areas.Identity.Pages.Account.Manage
{
    public class SubscriptionSuccessModel : PageModel
    {
        public DateTime ExpireDate { get; set; }

        public void OnGet(string expireDate)
        {
            if (DateTime.TryParse(expireDate, out var expire))
            {
                ExpireDate = expire;
            }
        }
    }
}
