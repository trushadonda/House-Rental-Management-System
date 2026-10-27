using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HouseRentalManagementSystem.Data;
using HouseRentalManagementSystem.Models;
using HouseRentalManagementSystem.Models.Enums;
using HouseRentalManagementSystem.ViewModels.Application;
using HouseRentalManagementSystem.ViewModels.Dashboard;

namespace HouseRentalManagementSystem.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Applications/Apply?propertyId=5
        [Authorize(Roles = "Tenant,Admin")]
        [HttpGet]
        public async Task<IActionResult> Apply(int? propertyId)
        {
            if (propertyId == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == propertyId && p.IsActive);

            if (property == null)
            {
                TempData["ErrorMessage"] = "Property not found or is no longer active.";
                return RedirectToAction("Index", "Properties");
            }

            if (!property.IsAvailable)
            {
                TempData["ErrorMessage"] = "This property is currently not available for new rental applications.";
                return RedirectToAction("Details", "Properties", new { id = propertyId });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Cannot apply for own property
            if (property.OwnerId == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot apply for your own property listing.";
                return RedirectToAction("Details", "Properties", new { id = propertyId });
            }

            // Check duplicate active application
            var existingApp = await _context.RentalApplications
                .FirstOrDefaultAsync(a => a.PropertyId == propertyId &&
                                         a.TenantId == currentUserId &&
                                         (a.Status == ApplicationStatus.Pending || a.Status == ApplicationStatus.Approved));

            if (existingApp != null)
            {
                TempData["InfoMessage"] = $"You already have an active application ({existingApp.Status}) for this property.";
                return RedirectToAction(nameof(MyApplications));
            }

            var model = new RentalApplicationCreateViewModel
            {
                PropertyId = property.Id,
                PropertyTitle = property.Title,
                PropertyAddress = $"{property.Address}, {property.City}, {property.State} {property.ZipCode}",
                PropertyCity = property.City,
                MonthlyRent = property.MonthlyRent,
                SecurityDeposit = property.SecurityDeposit,
                PropertyImageUrl = property.ImageUrl,
                OwnerName = property.Owner?.FullName ?? "Property Owner",
                ProposedMoveInDate = DateTime.Today.AddDays(7)
            };

            return View(model);
        }

        // POST: /Applications/Apply
        [Authorize(Roles = "Tenant,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(RentalApplicationCreateViewModel model)
        {
            var property = await _context.Properties
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == model.PropertyId && p.IsActive);

            if (property == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            // Prevent duplicate application
            var existingActive = await _context.RentalApplications
                .AnyAsync(a => a.PropertyId == model.PropertyId &&
                               a.TenantId == currentUserId &&
                               (a.Status == ApplicationStatus.Pending || a.Status == ApplicationStatus.Approved));

            if (existingActive)
            {
                TempData["ErrorMessage"] = "You already submitted an active application for this property.";
                return RedirectToAction(nameof(MyApplications));
            }

            if (!ModelState.IsValid)
            {
                model.PropertyTitle = property.Title;
                model.PropertyAddress = $"{property.Address}, {property.City}, {property.State} {property.ZipCode}";
                model.PropertyCity = property.City;
                model.MonthlyRent = property.MonthlyRent;
                model.SecurityDeposit = property.SecurityDeposit;
                model.PropertyImageUrl = property.ImageUrl;
                model.OwnerName = property.Owner?.FullName ?? "Property Owner";
                return View(model);
            }

            var application = new RentalApplication
            {
                PropertyId = model.PropertyId,
                TenantId = currentUserId,
                MonthlyIncome = model.MonthlyIncome,
                Occupation = model.Occupation.Trim(),
                NumberOfOccupants = model.NumberOfOccupants,
                ProposedMoveInDate = model.ProposedMoveInDate,
                Message = model.Message?.Trim(),
                Status = ApplicationStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            _context.RentalApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Your rental application for '{property.Title}' has been submitted successfully!";
            return RedirectToAction(nameof(MyApplications));
        }

        // GET: /Applications/MyApplications (Tenant Dashboard View)
        [Authorize(Roles = "Tenant,Admin")]
        [HttpGet]
        public async Task<IActionResult> MyApplications()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var applications = await _context.RentalApplications
                .Include(a => a.Property)
                    .ThenInclude(p => p!.Owner)
                .Where(a => a.TenantId == currentUserId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            var recommended = await _context.Properties
                .Where(p => p.IsActive && p.IsAvailable)
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .ToListAsync();

            var viewModel = new TenantDashboardViewModel
            {
                TotalApplications = applications.Count,
                PendingApplications = applications.Count(a => a.Status == ApplicationStatus.Pending),
                ApprovedApplications = applications.Count(a => a.Status == ApplicationStatus.Approved),
                RejectedApplications = applications.Count(a => a.Status == ApplicationStatus.Rejected),
                MyApplications = applications,
                RecommendedProperties = recommended
            };

            return View(viewModel);
        }

        // POST: /Applications/Cancel/5
        [Authorize(Roles = "Tenant,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var application = await _context.RentalApplications
                .Include(a => a.Property)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            // Security check
            if (application.TenantId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Unauthorized action.";
                return RedirectToAction(nameof(MyApplications));
            }

            // Can only cancel Pending applications
            if (application.Status != ApplicationStatus.Pending)
            {
                TempData["ErrorMessage"] = $"Cannot cancel this application because it is already {application.Status}.";
                return RedirectToAction(nameof(MyApplications));
            }

            application.Status = ApplicationStatus.Cancelled;
            application.ReviewedAt = DateTime.UtcNow;
            application.ReviewerRemarks = "Cancelled by applicant.";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your application has been cancelled.";
            return RedirectToAction(nameof(MyApplications));
        }

        // GET: /Applications/OwnerApplications
        [Authorize(Roles = "PropertyOwner,Admin")]
        [HttpGet]
        public async Task<IActionResult> OwnerApplications(int? propertyId, ApplicationStatus? status)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.RentalApplications
                .Include(a => a.Property)
                .Include(a => a.Tenant)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(a => a.Property != null && a.Property.OwnerId == currentUserId);
            }

            if (propertyId.HasValue && propertyId.Value > 0)
            {
                query = query.Where(a => a.PropertyId == propertyId.Value);
                ViewBag.SelectedPropertyId = propertyId.Value;
            }

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
                ViewBag.SelectedStatus = status.Value;
            }

            var applications = await query
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            // Also load owner's properties for filter dropdown
            var ownerProperties = await _context.Properties
                .Where(p => User.IsInRole("Admin") || p.OwnerId == currentUserId)
                .OrderBy(p => p.Title)
                .ToListAsync();

            ViewBag.OwnerProperties = ownerProperties;

            return View(applications);
        }

        // POST: /Applications/UpdateStatus
        [Authorize(Roles = "PropertyOwner,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int applicationId, ApplicationStatus status, string? reviewerRemarks)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var application = await _context.RentalApplications
                .Include(a => a.Property)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                return NotFound();
            }

            // Security: Ownership check
            if (application.Property == null || (application.Property.OwnerId != currentUserId && !User.IsInRole("Admin")))
            {
                TempData["ErrorMessage"] = "You do not have permission to manage this application.";
                return RedirectToAction(nameof(OwnerApplications));
            }

            application.Status = status;
            application.ReviewedAt = DateTime.UtcNow;
            application.ReviewerRemarks = reviewerRemarks?.Trim();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Application for {application.Tenant?.FullName ?? "Applicant"} has been marked as {status}.";
            return RedirectToAction(nameof(OwnerApplications));
        }

        // GET: /Applications/Details/5
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var application = await _context.RentalApplications
                .Include(a => a.Property)
                    .ThenInclude(p => p!.Owner)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            // Security: Only tenant, property owner, or admin can view
            bool isTenant = application.TenantId == currentUserId;
            bool isOwner = application.Property?.OwnerId == currentUserId;
            bool isAdmin = User.IsInRole("Admin");

            if (!isTenant && !isOwner && !isAdmin)
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CanReview = isOwner || isAdmin;
            return View(application);
        }
    }
}
