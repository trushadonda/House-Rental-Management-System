using System.Collections.Generic;
using HouseRentalManagementSystem.Models;

namespace HouseRentalManagementSystem.ViewModels.Property
{
    public class PropertySearchViewModel
    {
        public string? SearchQuery { get; set; }
        public string? City { get; set; }
        public string? PropertyType { get; set; }
        public int? Bedrooms { get; set; }
        public decimal? MinRent { get; set; }
        public decimal? MaxRent { get; set; }
        public string? SortBy { get; set; } = "newest"; // newest, rent_asc, rent_desc, area_desc

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalItems { get; set; }
        public int TotalPages => (int)System.Math.Ceiling((double)TotalItems / PageSize);

        public List<HouseRentalManagementSystem.Models.Property> Properties { get; set; } = new List<HouseRentalManagementSystem.Models.Property>();

        // Dropdown data
        public List<string> AvailableCities { get; set; } = new List<string>();
        public List<string> AvailableTypes { get; set; } = new List<string>();
    }
}

