using System.Collections.Generic;
using HouseRentalManagementSystem.Models;
using HouseRentalManagementSystem.Models.Enums;

namespace HouseRentalManagementSystem.ViewModels.Property
{
    public class PropertyDetailsViewModel
    {
        public HouseRentalManagementSystem.Models.Property Property { get; set; } = null!;
        public bool HasActiveApplication { get; set; }
        public ApplicationStatus? UserApplicationStatus { get; set; }
        public int? UserApplicationId { get; set; }
        public bool IsOwner { get; set; }
        public List<HouseRentalManagementSystem.Models.Property> SimilarProperties { get; set; } = new List<HouseRentalManagementSystem.Models.Property>();
    }
}

