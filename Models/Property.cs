using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseRentalManagementSystem.Models
{
    public class Property
    {
        [Key]
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

        [Required(ErrorMessage = "Zip code is required")]
        [StringLength(20)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Monthly rent is required")]
        [Range(100, 10000000, ErrorMessage = "Please enter a valid monthly rent amount")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monthly Rent ($)")]
        public decimal MonthlyRent { get; set; }

        [Required(ErrorMessage = "Security deposit is required")]
        [Range(0, 10000000, ErrorMessage = "Please enter a valid security deposit amount")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Security Deposit ($)")]
        public decimal SecurityDeposit { get; set; }

        [Required(ErrorMessage = "Number of bedrooms is required")]
        [Range(0, 20, ErrorMessage = "Bedrooms must be between 0 and 20")]
        public int Bedrooms { get; set; }

        [Required(ErrorMessage = "Number of bathrooms is required")]
        [Range(1, 20, ErrorMessage = "Bathrooms must be between 1 and 20")]
        public int Bathrooms { get; set; }

        [Required(ErrorMessage = "Area size is required")]
        [Range(50, 50000, ErrorMessage = "Area must be between 50 and 50,000 sq ft")]
        [Display(Name = "Area (Sq Ft)")]
        public int AreaSqFt { get; set; }

        [Required(ErrorMessage = "Property type is required")]
        [StringLength(50)]
        [Display(Name = "Property Type")]
        public string PropertyType { get; set; } = "Apartment"; // Apartment, Villa, Independent House, Studio, Condo

        [Required(ErrorMessage = "Furnishing status is required")]
        [StringLength(50)]
        [Display(Name = "Furnishing Status")]
        public string FurnishingStatus { get; set; } = "Semi-Furnished"; // Furnished, Semi-Furnished, Unfurnished

        [StringLength(500)]
        public string Amenities { get; set; } = string.Empty; // Comma separated

        [Display(Name = "Available For Rent")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Listing Active")]
        public bool IsActive { get; set; } = true; // For Admin moderation

        [Display(Name = "Featured Listing")]
        public bool IsFeatured { get; set; } = false;

        [MaxLength(500)]
        [Display(Name = "Main Image")]
        public string ImageUrl { get; set; } = "/images/properties/default-house.jpg";

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        [ForeignKey("OwnerId")]
        public virtual ApplicationUser? Owner { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public virtual ICollection<RentalApplication> Applications { get; set; } = new List<RentalApplication>();
    }
}

