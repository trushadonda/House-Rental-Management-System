using System;
using System.ComponentModel.DataAnnotations;

namespace HouseRentalManagementSystem.ViewModels.Application
{
    public class RentalApplicationCreateViewModel
    {
        [Required]
        public int PropertyId { get; set; }

        public string PropertyTitle { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public string PropertyCity { get; set; } = string.Empty;
        public decimal MonthlyRent { get; set; }
        public decimal SecurityDeposit { get; set; }
        public string? PropertyImageUrl { get; set; }
        public string OwnerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please state your monthly income")]
        [Range(100, 10000000, ErrorMessage = "Please enter a valid monthly income")]
        [Display(Name = "Monthly Income ($)")]
        public decimal MonthlyIncome { get; set; }

        [Required(ErrorMessage = "Please provide your occupation")]
        [StringLength(100)]
        [Display(Name = "Current Occupation / Employer")]
        public string Occupation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please specify number of occupants")]
        [Range(1, 20, ErrorMessage = "Number of occupants must be between 1 and 20")]
        [Display(Name = "Number of Occupants")]
        public int NumberOfOccupants { get; set; } = 1;

        [Required(ErrorMessage = "Please specify your intended move-in date")]
        [DataType(DataType.Date)]
        [Display(Name = "Proposed Move-In Date")]
        public DateTime ProposedMoveInDate { get; set; } = DateTime.Today.AddDays(7);

        [StringLength(1000)]
        [Display(Name = "Additional Notes / Message to Landlord")]
        public string? Message { get; set; }
    }
}

