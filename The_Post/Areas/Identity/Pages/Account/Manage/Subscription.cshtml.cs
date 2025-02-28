using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using The_Post.Models;
using The_Post.Services;
using Stripe.Checkout;

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

        // Property to hold current subscription type (if any)
        public SubscriptionType? CurrentSubscriptionType { get; set; }

        // GET: Loads the subscription page with available types.
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
                        
            // Get current subscription type if it exists
            CurrentSubscriptionType = await _subscriptionTypeService.GetCurrentSubscriptionTypeAsync(userId);
                       
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

            // Retrieve subscription type details
            var subscriptionType = await _subscriptionTypeService.GetByIdAsync(subscriptionTypeId);
            if (subscriptionType == null)
            {
                TempData["ErrorMessage"] = "Subscription type not found.";
                return RedirectToPage();
            }

            // Create the Stripe Checkout session options
            var domain = "https://localhost:7116";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(subscriptionType.Price * 100), // Price in the smallest currency unit
                            Currency = "sek", 
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = subscriptionType.TypeName,
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment", // For one-time payments
                SuccessUrl = domain + Url.Page("Subscription", new { handler = "Success", subscriptionTypeId }),
                CancelUrl = domain + Url.Page("Subscription", new { handler = "Cancel", subscriptionTypeId }),
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);
                        
            // Redirect to the Stripe Checkout page 
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);                                                
        }

        public async Task<IActionResult> OnGetSuccessAsync(int subscriptionTypeId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var subscription = await _subscriptionService.AddSubscription(userId, subscriptionTypeId);
            if (subscription == null)
            {
                TempData["ErrorMessage"] = "Failed to add subscription.";
                return RedirectToPage("Subscription");
            }
            // Redirect with the expiration date as a query parameter
            return RedirectToPage("SubscriptionSuccess", new { expireDate = subscription.Expires.ToString("yyyy-MM-dd") });
        }
        
        public IActionResult OnGetCancel(int subscriptionTypeId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            TempData["ErrorMessage"] = "Your payment was canceled. You can try subscribing again.";
                        
            return RedirectToPage("Subscription"); 
        }

        // POST: Handles canceling the subscription.
        public async Task<IActionResult> OnPostCancelAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _subscriptionService.CancelSubscriptionAsync(userId);
            if (!success)
            {
                TempData["ErrorMessage"] = "No active subscription found to cancel.";
            }
            else
            {
                TempData["SuccessMessage"] = "Your subscription has been canceled successfully.";
            }
            return RedirectToPage();
        }
    }
}
