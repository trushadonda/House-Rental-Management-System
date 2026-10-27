using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HouseRentalManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [Display(Name = "Role Type")]
        public string UserRole { get; set; } = "Tenant"; // Admin, PropertyOwner, Tenant

        [MaxLength(250)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Property> OwnedProperties { get; set; } = new List<Property>();
        public virtual ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();
    }
}

