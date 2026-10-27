using System;
using System.Collections.Generic;
using System.IO;
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
using HouseRentalManagementSystem.Services;
using HouseRentalManagementSystem.ViewModels.Property;

namespace HouseRentalManagementSystem.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService _fileStorageService;

        public PropertiesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _userManager = userManager;
            _fileStorageService = fileStorageService;
        }

        // GET: /Properties
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchQuery,
            string? city,
            string? propertyType,
            int? bedrooms,
            decimal? minRent,
            decimal? maxRent,
            string? sortBy = "newest",
            int pageNumber = 1)
        {
            var query = _context.Properties
                .Include(p => p.Owner)
                .Where(p => p.IsActive); // Only display active properties to the public

            // Apply Filters
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var term = searchQuery.Trim().ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    p.Address.ToLower().Contains(term) ||
                    p.City.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.City.ToLower() == city.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(propertyType))
            {
                query = query.Where(p => p.PropertyType.ToLower() == propertyType.Trim().ToLower());
            }

            if (bedrooms.HasValue && bedrooms.Value > 0)
            {
                if (bedrooms.Value >= 4)
                {
                    query = query.Where(p => p.Bedrooms >= 4);
                }
                else
                {
                    query = query.Where(p => p.Bedrooms == bedrooms.Value);
                }
            }

            if (minRent.HasValue && minRent.Value > 0)
            {
                query = query.Where(p => p.MonthlyRent >= minRent.Value);
            }

            if (maxRent.HasValue && maxRent.Value > 0)
            {
                query = query.Where(p => p.MonthlyRent <= maxRent.Value);
            }

            // Apply Sorting - Cast decimal to double for universal provider compatibility (SQL Server & SQLite)
            query = sortBy switch
            {
                "rent_asc" => query.OrderBy(p => (double)p.MonthlyRent),
                "rent_desc" => query.OrderByDescending(p => (double)p.MonthlyRent),
                "area_desc" => query.OrderByDescending(p => p.AreaSqFt),
                _ => query.OrderByDescending(p => p.CreatedAt) // default "newest"
            };

            var totalItems = await query.CountAsync();
            const int pageSize = 9;
            var safePageNumber = Math.Max(1, pageNumber);

            var items = await query
                .Skip((safePageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Populate filter dropdown lists
            var availableCities = await _context.Properties
                .Where(p => p.IsActive)
                .Select(p => p.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var availableTypes = new List<string> { "Apartment", "Villa", "Independent House", "Studio", "Condo" };

            var viewModel = new PropertySearchViewModel
            {
                SearchQuery = searchQuery,
                City = city,
                PropertyType = propertyType,
                Bedrooms = bedrooms,
                MinRent = minRent,
                MaxRent = maxRent,
                SortBy = sortBy,
                PageNumber = safePageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                Properties = items,
                AvailableCities = availableCities,
                AvailableTypes = availableTypes
            };

            return View(viewModel);
        }

        // GET: /Properties/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            // Admin can view any property; otherwise only active properties can be viewed
            if (!property.IsActive && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "This property listing is currently inactive.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new PropertyDetailsViewModel
            {
                Property = property
            };

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(currentUserId))
            {
                viewModel.IsOwner = property.OwnerId == currentUserId;

                // Check active application for current tenant
                var existingApp = await _context.RentalApplications
                    .FirstOrDefaultAsync(a => a.PropertyId == property.Id && a.TenantId == currentUserId);

                if (existingApp != null)
                {
                    viewModel.HasActiveApplication = true;
                    viewModel.UserApplicationStatus = existingApp.Status;
                    viewModel.UserApplicationId = existingApp.Id;
                }
            }

            // Similar properties recommendation
            viewModel.SimilarProperties = await _context.Properties
                .Where(p => p.Id != property.Id && p.IsActive && p.IsAvailable && (p.City == property.City || p.PropertyType == property.PropertyType))
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: /Properties/Create
        [Authorize(Roles = "PropertyOwner,Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new PropertyCreateEditViewModel
            {
                MonthlyRent = 1500,
                SecurityDeposit = 1500,
                Bedrooms = 2,
                Bathrooms = 1,
                AreaSqFt = 950,
                PropertyType = "Apartment",
                FurnishingStatus = "Semi-Furnished",
                IsAvailable = true
            };
            return View(model);
        }

        // POST: /Properties/Create
        [Authorize(Roles = "PropertyOwner,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            string primaryImageUrl = "/images/properties/default-house.jpg";

            // Upload primary image if provided
            if (model.MainImageFile != null && model.MainImageFile.Length > 0)
            {
                var uploadResult = await _fileStorageService.SaveImageAsync(model.MainImageFile, "properties");
                if (uploadResult.Success)
                {
                    primaryImageUrl = uploadResult.FilePath!;
                }
                else
                {
                    ModelState.AddModelError("MainImageFile", uploadResult.ErrorMessage ?? "Image upload failed");
                    return View(model);
                }
            }

            var amenitiesString = model.SelectedAmenities != null && model.SelectedAmenities.Count > 0
                ? string.Join(",", model.SelectedAmenities)
                : string.Empty;

            var property = new Property
            {
                Title = model.Title.Trim(),
                Description = model.Description.Trim(),
                Address = model.Address.Trim(),
                City = model.City.Trim(),
                State = model.State.Trim(),
                ZipCode = model.ZipCode.Trim(),
                MonthlyRent = model.MonthlyRent,
                SecurityDeposit = model.SecurityDeposit,
                Bedrooms = model.Bedrooms,
                Bathrooms = model.Bathrooms,
                AreaSqFt = model.AreaSqFt,
                PropertyType = model.PropertyType,
                FurnishingStatus = model.FurnishingStatus,
                Amenities = amenitiesString,
                IsAvailable = model.IsAvailable,
                IsActive = true,
                IsFeatured = false,
                ImageUrl = primaryImageUrl,
                OwnerId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            // Save additional gallery images if any
            if (model.AdditionalImageFiles != null && model.AdditionalImageFiles.Count > 0)
            {
                foreach (var addFile in model.AdditionalImageFiles)
                {
                    if (addFile.Length > 0)
                    {
                        var uploadRes = await _fileStorageService.SaveImageAsync(addFile, "properties");
                        if (uploadRes.Success)
                        {
                            _context.PropertyImages.Add(new PropertyImage
                            {
                                PropertyId = property.Id,
                                ImageUrl = uploadRes.FilePath!,
                                IsPrimary = false,
                                UploadedAt = DateTime.UtcNow
                            });
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Property '{property.Title}' listed successfully!";
            return RedirectToAction(nameof(MyProperties));
        }

        // GET: /Properties/Edit/5
        [Authorize(Roles = "PropertyOwner,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            // Security: Ownership check
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (property.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "You do not have permission to edit this property.";
                return RedirectToAction(nameof(MyProperties));
            }

            var selectedAmenities = !string.IsNullOrWhiteSpace(property.Amenities)
                ? property.Amenities.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList()
                : new List<string>();

            var model = new PropertyCreateEditViewModel
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                Address = property.Address,
                City = property.City,
                State = property.State,
                ZipCode = property.ZipCode,
                MonthlyRent = property.MonthlyRent,
                SecurityDeposit = property.SecurityDeposit,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                AreaSqFt = property.AreaSqFt,
                PropertyType = property.PropertyType,
                FurnishingStatus = property.FurnishingStatus,
                IsAvailable = property.IsAvailable,
                ExistingImageUrl = property.ImageUrl,
                SelectedAmenities = selectedAmenities,
                ExistingGalleryImages = property.Images.Select(i => i.ImageUrl).ToList()
            };

            return View(model);
        }

        // POST: /Properties/Edit/5
        [Authorize(Roles = "PropertyOwner,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyCreateEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            // Security: Ownership check
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (property.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Unauthorized edit attempt.";
                return RedirectToAction(nameof(MyProperties));
            }

            // Check if main image replaced
            if (model.MainImageFile != null && model.MainImageFile.Length > 0)
            {
                var uploadResult = await _fileStorageService.SaveImageAsync(model.MainImageFile, "properties");
                if (uploadResult.Success)
                {
                    // Optionally delete old local file
                    _fileStorageService.DeleteFile(property.ImageUrl);
                    property.ImageUrl = uploadResult.FilePath!;
                }
                else
                {
                    ModelState.AddModelError("MainImageFile", uploadResult.ErrorMessage ?? "Image upload failed");
                    return View(model);
                }
            }

            var amenitiesString = model.SelectedAmenities != null && model.SelectedAmenities.Count > 0
                ? string.Join(",", model.SelectedAmenities)
                : string.Empty;

            property.Title = model.Title.Trim();
            property.Description = model.Description.Trim();
            property.Address = model.Address.Trim();
            property.City = model.City.Trim();
            property.State = model.State.Trim();
            property.ZipCode = model.ZipCode.Trim();
            property.MonthlyRent = model.MonthlyRent;
            property.SecurityDeposit = model.SecurityDeposit;
            property.Bedrooms = model.Bedrooms;
            property.Bathrooms = model.Bathrooms;
            property.AreaSqFt = model.AreaSqFt;
            property.PropertyType = model.PropertyType;
            property.FurnishingStatus = model.FurnishingStatus;
            property.Amenities = amenitiesString;
            property.IsAvailable = model.IsAvailable;
            property.UpdatedAt = DateTime.UtcNow;

            // Handle additional gallery images
            if (model.AdditionalImageFiles != null && model.AdditionalImageFiles.Count > 0)
            {
                foreach (var addFile in model.AdditionalImageFiles)
                {
                    if (addFile.Length > 0)
                    {
                        var uploadRes = await _fileStorageService.SaveImageAsync(addFile, "properties");
                        if (uploadRes.Success)
                        {
                            _context.PropertyImages.Add(new PropertyImage
                            {
                                PropertyId = property.Id,
                                ImageUrl = uploadRes.FilePath!,
                                IsPrimary = false,
                                UploadedAt = DateTime.UtcNow
                            });
                        }
                    }
                }
            }

            _context.Properties.Update(property);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Listing '{property.Title}' updated successfully!";
            return RedirectToAction(nameof(MyProperties));
        }

        // POST: /Properties/ToggleAvailability/5
        [Authorize(Roles = "PropertyOwner,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (property.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Unauthorized action.";
                return RedirectToAction(nameof(MyProperties));
            }

            property.IsAvailable = !property.IsAvailable;
            property.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var statusText = property.IsAvailable ? "Available for Rent" : "Marked as Rented / Unavailable";
            TempData["SuccessMessage"] = $"Property '{property.Title}' is now {statusText}.";

            return RedirectToAction(nameof(MyProperties));
        }

        // POST: /Properties/Delete/5
        [Authorize(Roles = "PropertyOwner,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (property.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Unauthorized delete attempt.";
                return RedirectToAction(nameof(MyProperties));
            }

            // Cleanup local images
            _fileStorageService.DeleteFile(property.ImageUrl);
            foreach (var img in property.Images)
            {
                _fileStorageService.DeleteFile(img.ImageUrl);
            }

            var title = property.Title;
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Property '{title}' has been successfully deleted.";
            return RedirectToAction(nameof(MyProperties));
        }

        // GET: /Properties/MyProperties
        [Authorize(Roles = "PropertyOwner,Admin")]
        [HttpGet]
        public async Task<IActionResult> MyProperties()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            var query = _context.Properties
                .Include(p => p.Applications)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(p => p.OwnerId == currentUserId);
            }

            var properties = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var pendingAppsCount = await _context.RentalApplications
                .Where(a => a.Property != null && a.Property.OwnerId == currentUserId && a.Status == ApplicationStatus.Pending)
                .CountAsync();

            ViewBag.PendingApplicationsCount = pendingAppsCount;
            return View(properties);
        }
    }
}
