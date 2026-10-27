using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HouseRentalManagementSystem.ViewModels.Property
{
    public class PropertyCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Property title is required")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Property description is required")]
        [StringLength(4000, ErrorMessage = "Description cannot exceed 4000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street address is required")]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zip Code is required")]
        [StringLength(20)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Monthly rent is required")]
        [Range(50, 1000000, ErrorMessage = "Please enter a valid monthly rent amount ($50 - $1,000,000)")]
        [Display(Name = "Monthly Rent ($)")]
        public decimal MonthlyRent { get; set; }

        [Required(ErrorMessage = "Security deposit is required")]
        [Range(0, 1000000, ErrorMessage = "Please enter a valid security deposit amount")]
        [Display(Name = "Security Deposit ($)")]
        public decimal SecurityDeposit { get; set; }

        [Required(ErrorMessage = "Bedrooms count is required")]
        [Range(0, 20, ErrorMessage = "Bedrooms must be between 0 and 20")]
        public int Bedrooms { get; set; } = 1;

        [Required(ErrorMessage = "Bathrooms count is required")]
        [Range(1, 20, ErrorMessage = "Bathrooms must be between 1 and 20")]
        public int Bathrooms { get; set; } = 1;

        [Required(ErrorMessage = "Area size is required")]
        [Range(50, 50000, ErrorMessage = "Area must be between 50 and 50,000 sq ft")]
        [Display(Name = "Area (Sq Ft)")]
        public int AreaSqFt { get; set; } = 750;

        [Required(ErrorMessage = "Property type is required")]
        [Display(Name = "Property Type")]
        public string PropertyType { get; set; } = "Apartment";

        [Required(ErrorMessage = "Furnishing status is required")]
        [Display(Name = "Furnishing Status")]
        public string FurnishingStatus { get; set; } = "Semi-Furnished";

        [Display(Name = "Amenities")]
        public List<string> SelectedAmenities { get; set; } = new List<string>();

        [Display(Name = "Available For Rent Now")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Current Primary Image")]
        public string? ExistingImageUrl { get; set; }

        [Display(Name = "Upload Main Image")]
        public IFormFile? MainImageFile { get; set; }

        [Display(Name = "Additional Images (Optional)")]
        public List<IFormFile>? AdditionalImageFiles { get; set; }

        // Existing additional gallery images
        public List<string> ExistingGalleryImages { get; set; } = new List<string>();
    }
}

