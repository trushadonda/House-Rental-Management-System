using System.Collections.Generic;
using HouseRentalManagementSystem.Models;

namespace HouseRentalManagementSystem.ViewModels.Dashboard
{
    public class TenantDashboardViewModel
    {
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }

        public List<HouseRentalManagementSystem.Models.RentalApplication> MyApplications { get; set; } = new List<HouseRentalManagementSystem.Models.RentalApplication>();
        public List<HouseRentalManagementSystem.Models.Property> RecommendedProperties { get; set; } = new List<HouseRentalManagementSystem.Models.Property>();
    }
}

