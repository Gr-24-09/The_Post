using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using The_Post.Models;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class SubscriptionTypesController : Controller
    {
        private readonly ISubscriptionTypeService _subscriptionTypeService;

        public SubscriptionTypesController(ISubscriptionTypeService subscriptionTypeService)
        {
            _subscriptionTypeService = subscriptionTypeService;
        }
               
        public async Task<IActionResult> Index()
        {
            var subTypes = await _subscriptionTypeService.GetAllSubTypes();
            return View(subTypes);
        }
                
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                throw new ArgumentException("Id not found", nameof(id));
            }

            var subType = await _subscriptionTypeService.GetByIdAsync(id.Value);
            if (subType == null)
            {
                throw new ArgumentException("SubscriptionType not found", nameof(subType));
            }

            return View(subType);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
               
        [HttpPost]        
        public async Task<IActionResult> Create([Bind("Id,TypeName,Description,Price")] SubscriptionType subType)
        {
            if (ModelState.IsValid)
            {
                await _subscriptionTypeService.CreateSubType(subType);
                return RedirectToAction(nameof(Index));
            }
            return View(subType);
        }
                
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                throw new ArgumentException("Id not found", nameof(id));
            }

            var subType = await _subscriptionTypeService.GetByIdAsync(id.Value);
            if (subType == null)
            {
                throw new ArgumentException("SubscriptionType not found", nameof(subType));
            }
            return View(subType);
        }
                
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeName,Description,Price")] SubscriptionType subType)
        {
            if (id != subType.Id)
            {
                throw new ArgumentException("Id not found", nameof(id));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _subscriptionTypeService.EditSubType(subType);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _subscriptionTypeService.Exists(subType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subType);
        }
               
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                throw new ArgumentException("Id not found", nameof(id));
            }

            var subType = await _subscriptionTypeService.GetByIdAsync(id.Value);
            if (subType == null)
            {
                return NotFound();
            }

            return View(subType);
        }
                
        [HttpPost]       
        public async Task<IActionResult> Delete(int id)
        {
            await _subscriptionTypeService.DeleteSubType(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
