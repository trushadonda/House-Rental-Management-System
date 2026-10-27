using System.Collections.Generic;
using HouseRentalManagementSystem.Models;

namespace HouseRentalManagementSystem.ViewModels.Dashboard
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalOwners { get; set; }
        public int TotalTenants { get; set; }
        public int TotalProperties { get; set; }
        public int AvailableProperties { get; set; }
        public int InactiveProperties { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }

        public List<HouseRentalManagementSystem.Models.Property> RecentProperties { get; set; } = new List<HouseRentalManagementSystem.Models.Property>();
        public List<HouseRentalManagementSystem.Models.RentalApplication> RecentApplications { get; set; } = new List<HouseRentalManagementSystem.Models.RentalApplication>();
        public List<HouseRentalManagementSystem.Models.ApplicationUser> RecentUsers { get; set; } = new List<HouseRentalManagementSystem.Models.ApplicationUser>();
    }
}

