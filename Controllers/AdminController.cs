using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HouseRentalManagementSystem.Data;
using HouseRentalManagementSystem.Models;
using HouseRentalManagementSystem.Models.Enums;
using HouseRentalManagementSystem.ViewModels.Dashboard;

namespace HouseRentalManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalOwners = await _context.Users.CountAsync(u => u.UserRole == "PropertyOwner");
            var totalTenants = await _context.Users.CountAsync(u => u.UserRole == "Tenant");

            var totalProperties = await _context.Properties.CountAsync();
            var availableProperties = await _context.Properties.CountAsync(p => p.IsAvailable && p.IsActive);
            var inactiveProperties = await _context.Properties.CountAsync(p => !p.IsActive);

            var totalApps = await _context.RentalApplications.CountAsync();
            var pendingApps = await _context.RentalApplications.CountAsync(a => a.Status == ApplicationStatus.Pending);
            var approvedApps = await _context.RentalApplications.CountAsync(a => a.Status == ApplicationStatus.Approved);

            var recentProps = await _context.Properties
                .Include(p => p.Owner)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentApps = await _context.RentalApplications
                .Include(a => a.Property)
                .Include(a => a.Tenant)
                .OrderByDescending(a => a.AppliedAt)
                .Take(5)
                .ToListAsync();

            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalOwners = totalOwners,
                TotalTenants = totalTenants,
                TotalProperties = totalProperties,
                AvailableProperties = availableProperties,
                InactiveProperties = inactiveProperties,
                TotalApplications = totalApps,
                PendingApplications = pendingApps,
                ApprovedApplications = approvedApps,
                RecentProperties = recentProps,
                RecentApplications = recentApps,
                RecentUsers = recentUsers
            };

            return View(model);
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users(string? roleFilter, string? search)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                query = query.Where(u => u.UserRole == roleFilter);
                ViewBag.SelectedRole = roleFilter;
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(term) || (u.Email != null && u.Email.ToLower().Contains(term)));
                ViewBag.SearchTerm = search;
            }

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
            return View(users);
        }

        // POST: /Admin/ToggleUserStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent admin from deactivating self
            var currentAdmin = await _userManager.GetUserAsync(User);
            if (currentAdmin != null && currentAdmin.Id == user.Id)
            {
                TempData["ErrorMessage"] = "You cannot deactivate your own administrator account.";
                return RedirectToAction(nameof(Users));
            }

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            var statusStr = user.IsActive ? "activated" : "deactivated";
            TempData["SuccessMessage"] = $"User {user.FullName} ({user.Email}) has been {statusStr}.";

            return RedirectToAction(nameof(Users));
        }

        // GET: /Admin/Properties
        public async Task<IActionResult> Properties(string? statusFilter, string? search)
        {
            var query = _context.Properties
                .Include(p => p.Owner)
                .AsQueryable();

            if (statusFilter == "active")
            {
                query = query.Where(p => p.IsActive);
            }
            else if (statusFilter == "inactive")
            {
                query = query.Where(p => !p.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(term) || p.City.ToLower().Contains(term) || (p.Owner != null && p.Owner.FullName.ToLower().Contains(term)));
                ViewBag.SearchTerm = search;
            }

            ViewBag.SelectedStatus = statusFilter;

            var properties = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(properties);
        }

        // POST: /Admin/TogglePropertyActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePropertyActive(int propertyId)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            property.IsActive = !property.IsActive;
            property.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var statusStr = property.IsActive ? "activated" : "deactivated";
            TempData["SuccessMessage"] = $"Property '{property.Title}' has been {statusStr}.";

            return RedirectToAction(nameof(Properties));
        }

        // GET: /Admin/Applications
        public async Task<IActionResult> Applications(ApplicationStatus? statusFilter)
        {
            var query = _context.RentalApplications
                .Include(a => a.Property)
                .Include(a => a.Tenant)
                .AsQueryable();

            if (statusFilter.HasValue)
            {
                query = query.Where(a => a.Status == statusFilter.Value);
                ViewBag.SelectedStatus = statusFilter.Value;
            }

            var applications = await query.OrderByDescending(a => a.AppliedAt).ToListAsync();
            return View(applications);
        }
    }
}

