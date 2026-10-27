using System.ComponentModel.DataAnnotations;
using HouseRentalManagementSystem.Models.Enums;

namespace HouseRentalManagementSystem.ViewModels.Application
{
    public class ApplicationStatusUpdateViewModel
    {
        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public ApplicationStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters")]
        [Display(Name = "Remarks / Notes for Tenant")]
        public string? ReviewerRemarks { get; set; }
    }
}

