using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HouseRentalManagementSystem.Models;
using HouseRentalManagementSystem.Models.Enums;

namespace HouseRentalManagementSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Apply pending migrations or ensure database created
            await context.Database.EnsureCreatedAsync();

            // 1. Seed Roles
            string[] roles = { "Admin", "PropertyOwner", "Tenant" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Default Admin User
            var adminEmail = "admin@houserental.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    UserRole = "Admin",
                    PhoneNumber = "+1 555-0100",
                    Address = "Corporate Tower, Suite 400, New York, NY",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6),
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. Seed Default Property Owner
            var ownerEmail = "owner@houserental.com";
            var ownerUser = await userManager.FindByEmailAsync(ownerEmail);
            if (ownerUser == null)
            {
                ownerUser = new ApplicationUser
                {
                    UserName = ownerEmail,
                    Email = ownerEmail,
                    FullName = "Marcus Vance (Premier Properties)",
                    UserRole = "PropertyOwner",
                    PhoneNumber = "+1 555-0199",
                    Address = "742 Evergreen Terrace, Springfield, IL",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3),
                    IsActive = true
                };

                var result = await userManager.CreateAsync(ownerUser, "Owner@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(ownerUser, "PropertyOwner");
                }
            }

            // 4. Seed Default Tenant
            var tenantEmail = "tenant@houserental.com";
            var tenantUser = await userManager.FindByEmailAsync(tenantEmail);
            if (tenantUser == null)
            {
                tenantUser = new ApplicationUser
                {
                    UserName = tenantEmail,
                    Email = tenantEmail,
                    FullName = "Elena Rostova",
                    UserRole = "Tenant",
                    PhoneNumber = "+1 555-0145",
                    Address = "120 Elmwood St, Chicago, IL",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1),
                    IsActive = true
                };

                var result = await userManager.CreateAsync(tenantUser, "Tenant@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(tenantUser, "Tenant");
                }
            }

            // 5. Seed Properties if none exist
            if (!await context.Properties.AnyAsync())
            {
                var sampleProperties = new List<Property>
                {
                    new Property
                    {
                        Title = "Serene Oak Luxury Penthouse",
                        Description = "Experience luxury living in this spacious 3-bedroom penthouse offering panoramic city skyline views, marble countertops, high ceilings, automated climate control, and a massive private sun terrace. Perfect for professionals and modern families seeking comfort and elegance.",
                        Address = "104 Lincoln Avenue, Suite 18A",
                        City = "Chicago",
                        State = "IL",
                        ZipCode = "60601",
                        MonthlyRent = 3200.00m,
                        SecurityDeposit = 3200.00m,
                        Bedrooms = 3,
                        Bathrooms = 3,
                        AreaSqFt = 2100,
                        PropertyType = "Apartment",
                        FurnishingStatus = "Furnished",
                        Amenities = "WiFi,Parking,Air Conditioning,Balcony,Swimming Pool,Gym,Power Backup,Security,Elevator",
                        IsAvailable = true,
                        IsActive = true,
                        IsFeatured = true,
                        ImageUrl = "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?auto=format&fit=crop&w=1000&q=80",
                        OwnerId = ownerUser!.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new Property
                    {
                        Title = "Modern Emerald Haven Villa",
                        Description = "Stunning contemporary 4-bedroom villa with a private landscaped garden, heated swimming pool, dual-car garage, and top-tier smart home security. Situated in a tranquil gated community with easy access to premier international schools and shopping hubs.",
                        Address = "45 Magnolia Blossom Way",
                        City = "Austin",
                        State = "TX",
                        ZipCode = "78701",
                        MonthlyRent = 4500.00m,
                        SecurityDeposit = 5000.00m,
                        Bedrooms = 4,
                        Bathrooms = 4,
                        AreaSqFt = 3400,
                        PropertyType = "Villa",
                        FurnishingStatus = "Furnished",
                        Amenities = "WiFi,Parking,Air Conditioning,Balcony,Swimming Pool,Gym,Power Backup,Security,Pet Friendly",
                        IsAvailable = true,
                        IsActive = true,
                        IsFeatured = true,
                        ImageUrl = "https://images.unsplash.com/photo-1613977257363-707ba9348227?auto=format&fit=crop&w=1000&q=80",
                        OwnerId = ownerUser.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    },
                    new Property
                    {
                        Title = "Sunlit Scandinavian Studio",
                        Description = "Bright, thoughtfully organized studio apartment equipped with modular furniture, hardwood floors, high-speed fiber internet, and full kitchen amenities. Located within walking distance of central transit stations, boutique coffee shops, and local parks.",
                        Address = "218 Market Street, Unit 4B",
                        City = "Seattle",
                        State = "WA",
                        ZipCode = "98101",
                        MonthlyRent = 1650.00m,
                        SecurityDeposit = 1650.00m,
                        Bedrooms = 1,
                        Bathrooms = 1,
                        AreaSqFt = 620,
                        PropertyType = "Studio",
                        FurnishingStatus = "Furnished",
                        Amenities = "WiFi,Air Conditioning,Elevator,Security,Power Backup",
                        IsAvailable = true,
                        IsActive = true,
                        IsFeatured = true,
                        ImageUrl = "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?auto=format&fit=crop&w=1000&q=80",
                        OwnerId = ownerUser.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Property
                    {
                        Title = "Craftsman Family Independent Home",
                        Description = "Charming 3-bedroom detached home featuring a covered front porch, renovated chef's kitchen with stainless steel appliances, backyard deck for grilling, and a spacious master suite. Family-friendly neighborhood close to parks and recreational trails.",
                        Address = "88 Pinecrest Boulevard",
                        City = "Denver",
                        State = "CO",
                        ZipCode = "80202",
                        MonthlyRent = 2800.00m,
                        SecurityDeposit = 2800.00m,
                        Bedrooms = 3,
                        Bathrooms = 2,
                        AreaSqFt = 1850,
                        PropertyType = "Independent House",
                        FurnishingStatus = "Semi-Furnished",
                        Amenities = "Parking,Air Conditioning,Balcony,Power Backup,Pet Friendly,Security",
                        IsAvailable = true,
                        IsActive = true,
                        IsFeatured = false,
                        ImageUrl = "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=1000&q=80",
                        OwnerId = ownerUser.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-8)
                    },
                    new Property
                    {
                        Title = "Harbor View Coastal Condo",
                        Description = "Contemporary 2-bedroom condominium with stunning waterfront vistas, private balcony, walk-in closets, in-unit laundry, and access to a rooftop lounge and fitness studio. Secure access building with concierge service.",
                        Address = "330 Ocean Drive, Apt 702",
                        City = "San Diego",
                        State = "CA",
                        ZipCode = "92101",
                        MonthlyRent = 3100.00m,
                        SecurityDeposit = 3100.00m,
                        Bedrooms = 2,
                        Bathrooms = 2,
                        AreaSqFt = 1350,
                        PropertyType = "Condo",
                        FurnishingStatus = "Semi-Furnished",
                        Amenities = "WiFi,Parking,Air Conditioning,Balcony,Swimming Pool,Gym,Elevator,Security",
                        IsAvailable = true,
                        IsActive = true,
                        IsFeatured = true,
                        ImageUrl = "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?auto=format&fit=crop&w=1000&q=80",
                        OwnerId = ownerUser.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new Property
                    {
                        Title = "Chic Downtown Loft",
                        Description = "Exposed brick, soaring 14-foot timber ceilings, and industrial design accents define this stylish 2-bedroom downtown loft. Includes smart thermostat, quartz kitchen island, and designated underground parking spot.",
                        Address = "510 Broadway Avenue, Unit 302",
                        City = "Chicago",
                        State = "IL",
                        ZipCode = "60611",
                        MonthlyRent = 2650.00m,
                        SecurityDeposit = 2650.00m,
                        Bedrooms = 2,
                        Bathrooms = 2,
                        AreaSqFt = 1250,
                        PropertyType = "Apartment",
                        FurnishingStatus = "Furnished",
                        Amenities = "WiFi,Parking,Air Conditioning,Gym,Elevator,Security",
                        IsAvailable = true,
                        IsActive = true,
                        IsFeatured = false,
                        ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=1000&q=80",
                        OwnerId = ownerUser.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    }
                };

                await context.Properties.AddRangeAsync(sampleProperties);
                await context.SaveChangesAsync();

                // 6. Seed sample applications for tenant
                var firstProp = sampleProperties[0];
                var secondProp = sampleProperties[1];

                var sampleApplications = new List<RentalApplication>
                {
                    new RentalApplication
                    {
                        PropertyId = firstProp.Id,
                        TenantId = tenantUser!.Id,
                        MonthlyIncome = 7800.00m,
                        Occupation = "Senior AI Software Engineer",
                        NumberOfOccupants = 2,
                        ProposedMoveInDate = DateTime.Today.AddDays(14),
                        Message = "Hello Marcus, my partner and I are relocating for work and absolutely adore the penthouse terrace and natural lighting. We maintain excellent credit and non-smoking habits.",
                        Status = ApplicationStatus.Pending,
                        AppliedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new RentalApplication
                    {
                        PropertyId = secondProp.Id,
                        TenantId = tenantUser.Id,
                        MonthlyIncome = 7800.00m,
                        Occupation = "Senior AI Software Engineer",
                        NumberOfOccupants = 2,
                        ProposedMoveInDate = DateTime.Today.AddDays(25),
                        Message = "Interested in this lovely villa for a long-term lease. Can provide landlord references upon request.",
                        Status = ApplicationStatus.Approved,
                        AppliedAt = DateTime.UtcNow.AddDays(-7),
                        ReviewedAt = DateTime.UtcNow.AddDays(-5),
                        ReviewerRemarks = "Application approved! Verified credit profile and employment history. Welcoming you to Emerald Haven."
                    }
                };

                await context.RentalApplications.AddRangeAsync(sampleApplications);
                await context.SaveChangesAsync();
            }
        }
    }
}

