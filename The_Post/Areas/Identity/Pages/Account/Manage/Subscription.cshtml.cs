using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using The_Post.Models;
using The_Post.Services;

namespace The_Post.Areas.Identity.Pages.Account.Manage
{
    public class SubscriptionModel : PageModel
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly UserManager<User> _userManager;

        public SubscriptionModel(ISubscriptionService subscriptionService, ISubscriptionTypeService subscriptionTypeService, UserManager<User> userManager)
        {
            _subscriptionService = subscriptionService;
            _subscriptionTypeService = subscriptionTypeService;
            _userManager = userManager;
        }

        // Property to hold available subscription types for the view.
        public IEnumerable<SubscriptionType> SubscriptionTypes { get; set; } = new List<SubscriptionType>();

        // GET: Loads the subscription page with available types.
        public async Task<IActionResult> OnGetAsync()
        {
            SubscriptionTypes = await _subscriptionTypeService.GetAllSubscriptionTypes();
            return Page();
        }

        // POST: Handles subscribing
        public async Task<IActionResult> OnPostSubscribeAsync(int subscriptionTypeId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool result = await _subscriptionService.AddSubscription(userId, subscriptionTypeId);
            if (result)
            {
                TempData["SuccessMessage"] = "You have successfully subscribed!";
            }
            else
            {
                TempData["ErrorMessage"] = "There was a problem processing your subscription. Please try again.";
            }

            // Redirect to the same page
            return RedirectToPage();
        }

        // POST: Handles renewing the subscription.
        public async Task<IActionResult> OnPostRenewAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool result = await _subscriptionService.RenewSubscription(userId);
            if (result)
            {
                TempData["SuccessMessage"] = "Your subscription has been renewed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to renew your subscription. Please try again.";
            }
            return RedirectToPage();
        }

        // POST: Handles canceling the subscription.
        public async Task<IActionResult> OnPostCancelAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool result = await _subscriptionService.CancelSubscriptionAsync(userId);
            if (result)
            {
                TempData["SuccessMessage"] = "Your subscription has been canceled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel your subscription. Please try again.";
            }
            return RedirectToPage();
        }
    }
}
