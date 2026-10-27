using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HouseRentalManagementSystem.Data;
using HouseRentalManagementSystem.Models;
using HouseRentalManagementSystem.ViewModels.Home;

namespace HouseRentalManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /
        public async Task<IActionResult> Index()
        {
            var activePropertiesQuery = _context.Properties
                .Include(p => p.Owner)
                .Where(p => p.IsActive);

            var featured = await activePropertiesQuery
                .Where(p => p.IsFeatured && p.IsAvailable)
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToListAsync();

            var recent = await activePropertiesQuery
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToListAsync();

            var allCities = await activePropertiesQuery
                .GroupBy(p => new { p.City, p.State })
                .Select(g => new { g.Key.City, g.Key.State, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(4)
                .ToListAsync();

            // Default location pictures mapping
            var locationImages = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Chicago", "https://images.unsplash.com/photo-1494522855154-9297ac14b55f?auto=format&fit=crop&w=600&q=80" },
                { "Austin", "https://images.unsplash.com/photo-1531218150217-54595bc2b934?auto=format&fit=crop&w=600&q=80" },
                { "Seattle", "https://images.unsplash.com/photo-1502175353174-a7a70e73b362?auto=format&fit=crop&w=600&q=80" },
                { "San Diego", "https://images.unsplash.com/photo-1538688525198-9b88f6f53126?auto=format&fit=crop&w=600&q=80" },
                { "Denver", "https://images.unsplash.com/photo-1546156929-a4c0ac411f47?auto=format&fit=crop&w=600&q=80" },
                { "New York", "https://images.unsplash.com/photo-1496442226666-8d4d0e62e6e9?auto=format&fit=crop&w=600&q=80" }
            };

            var popularLocations = allCities.Select(c => new LocationStatItem
            {
                City = c.City,
                State = c.State,
                PropertyCount = c.Count,
                ImageUrl = locationImages.ContainsKey(c.City)
                    ? locationImages[c.City]
                    : "https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?auto=format&fit=crop&w=600&q=80"
            }).ToList();

            // If few cities in db, fallback to popular cities list
            if (popularLocations.Count == 0)
            {
                popularLocations.Add(new LocationStatItem { City = "Chicago", State = "IL", PropertyCount = 12, ImageUrl = "https://images.unsplash.com/photo-1494522855154-9297ac14b55f?auto=format&fit=crop&w=600&q=80" });
                popularLocations.Add(new LocationStatItem { City = "Austin", State = "TX", PropertyCount = 8, ImageUrl = "https://images.unsplash.com/photo-1531218150217-54595bc2b934?auto=format&fit=crop&w=600&q=80" });
                popularLocations.Add(new LocationStatItem { City = "Seattle", State = "WA", PropertyCount = 9, ImageUrl = "https://images.unsplash.com/photo-1502175353174-a7a70e73b362?auto=format&fit=crop&w=600&q=80" });
                popularLocations.Add(new LocationStatItem { City = "San Diego", State = "CA", PropertyCount = 7, ImageUrl = "https://images.unsplash.com/photo-1538688525198-9b88f6f53126?auto=format&fit=crop&w=600&q=80" });
            }

            var categoryCounts = await activePropertiesQuery
                .GroupBy(p => p.PropertyType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.Type, v => v.Count);

            var categories = new System.Collections.Generic.List<CategoryStatItem>
            {
                new CategoryStatItem { Name = "Apartment", IconClass = "bi-building", Description = "Modern high-rise & city living", Count = categoryCounts.ContainsKey("Apartment") ? categoryCounts["Apartment"] : 0 },
                new CategoryStatItem { Name = "Villa", IconClass = "bi-house-heart", Description = "Exclusive estates with private yards", Count = categoryCounts.ContainsKey("Villa") ? categoryCounts["Villa"] : 0 },
                new CategoryStatItem { Name = "Independent House", IconClass = "bi-house-door", Description = "Spacious single-family detached homes", Count = categoryCounts.ContainsKey("Independent House") ? categoryCounts["Independent House"] : 0 },
                new CategoryStatItem { Name = "Studio", IconClass = "bi-lamp", Description = "Compact, cozy & efficient spaces", Count = categoryCounts.ContainsKey("Studio") ? categoryCounts["Studio"] : 0 },
                new CategoryStatItem { Name = "Condo", IconClass = "bi-water", Description = "Premium amenities & scenic vistas", Count = categoryCounts.ContainsKey("Condo") ? categoryCounts["Condo"] : 0 }
            };

            var totalProps = await _context.Properties.CountAsync();
            var totalTenants = await _context.Users.CountAsync(u => u.UserRole == "Tenant");
            var totalCities = await _context.Properties.Select(p => p.City).Distinct().CountAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedProperties = featured,
                RecentProperties = recent,
                PopularLocations = popularLocations,
                Categories = categories,
                TotalPropertiesCount = totalProps,
                TotalTenantsCount = totalTenants,
                TotalCitiesCount = totalCities > 0 ? totalCities : 6
            };

            return View(viewModel);
        }

        // POST: /Home/ContactMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactMessage(string name, string email, string phone, string subject, string message)
        {
            // Simulate receiving contact inquiry
            TempData["SuccessMessage"] = $"Thank you, {name}! Your message regarding '{subject}' has been received. Our rental concierge team will contact you at {email} within 24 hours.";
            return RedirectToAction("Index", "Home", null, "contact-section");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
