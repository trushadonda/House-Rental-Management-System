using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HouseRentalManagementSystem.Models.Enums;

namespace HouseRentalManagementSystem.Models
{
    public class RentalApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }

        [Required]
        public string TenantId { get; set; } = string.Empty;

        [ForeignKey("TenantId")]
        public virtual ApplicationUser? Tenant { get; set; }

        [Required(ErrorMessage = "Monthly income is required")]
        [Range(100, 10000000, ErrorMessage = "Please enter a valid monthly income")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monthly Income ($)")]
        public decimal MonthlyIncome { get; set; }

        [Required(ErrorMessage = "Occupation is required")]
        [StringLength(100)]
        public string Occupation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of occupants is required")]
        [Range(1, 20, ErrorMessage = "Occupants must be at least 1")]
        [Display(Name = "Number of Occupants")]
        public int NumberOfOccupants { get; set; } = 1;

        [Required(ErrorMessage = "Proposed move-in date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Proposed Move-In Date")]
        public DateTime ProposedMoveInDate { get; set; } = DateTime.Today.AddDays(7);

        [StringLength(1000)]
        [Display(Name = "Message / Notes to Landlord")]
        public string? Message { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        [StringLength(500)]
        [Display(Name = "Owner Remarks")]
        public string? ReviewerRemarks { get; set; }
    }
}

