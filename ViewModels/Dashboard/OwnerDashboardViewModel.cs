using System.Collections.Generic;
using HouseRentalManagementSystem.Models;

namespace HouseRentalManagementSystem.ViewModels.Dashboard
{
    public class OwnerDashboardViewModel
    {
        public int TotalProperties { get; set; }
        public int AvailableProperties { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }

        public List<HouseRentalManagementSystem.Models.Property> MyProperties { get; set; } = new List<HouseRentalManagementSystem.Models.Property>();
        public List<HouseRentalManagementSystem.Models.RentalApplication> RecentApplications { get; set; } = new List<HouseRentalManagementSystem.Models.RentalApplication>();
    }
}

