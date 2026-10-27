using System;
using System.ComponentModel.DataAnnotations;

namespace HouseRentalManagementSystem.ViewModels.Account
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(250)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        public string Role { get; set; } = string.Empty;

        public DateTime MemberSince { get; set; }
    }
}

