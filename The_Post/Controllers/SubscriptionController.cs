using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using The_Post.Models;
using The_Post.Services;

namespace The_Post.Controllers
{
    //[Authorize] Only logged-in users can access these actions.
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly UserManager<User> _userManager;

        public SubscriptionController(ISubscriptionService subscriptionService, ISubscriptionTypeService subscriptionTypeService, UserManager<User> userManager)
        {
            _subscriptionService = subscriptionService;
            _subscriptionTypeService = subscriptionTypeService;
            _userManager = userManager;
        }
                
        // Displays all available subscription types.
        public async Task<IActionResult> Index()
        {
            var subscriptionTypes = await _subscriptionTypeService.GetAllSubscriptionTypes();
            return View(subscriptionTypes);
        }
                
        // Handles the subscription process for a given subscription type.
        [HttpPost]        
        public async Task<IActionResult> Subscribe(int subscriptionTypeId)
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
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "There was a problem processing your subscription. Please try again.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Renew()
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
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public async Task<IActionResult> Cancel()
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
            return RedirectToAction("Index", "Home");
        }
    }
}
