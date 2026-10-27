using System.Collections.Generic;
using HouseRentalManagementSystem.Models;

namespace HouseRentalManagementSystem.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<HouseRentalManagementSystem.Models.Property> FeaturedProperties { get; set; } = new List<HouseRentalManagementSystem.Models.Property>();
        public List<HouseRentalManagementSystem.Models.Property> RecentProperties { get; set; } = new List<HouseRentalManagementSystem.Models.Property>();
        public List<LocationStatItem> PopularLocations { get; set; } = new List<LocationStatItem>();
        public List<CategoryStatItem> Categories { get; set; } = new List<CategoryStatItem>();

        public int TotalPropertiesCount { get; set; }
        public int TotalTenantsCount { get; set; }
        public int TotalCitiesCount { get; set; }
    }

    public class LocationStatItem
    {
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int PropertyCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class CategoryStatItem
    {
        public string Name { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

