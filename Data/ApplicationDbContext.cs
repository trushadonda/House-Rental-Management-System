using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HouseRentalManagementSystem.Models;

namespace HouseRentalManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<PropertyImage> PropertyImages { get; set; } = null!;
        public DbSet<RentalApplication> RentalApplications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Property - Owner relationship
            builder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedProperties)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Property - PropertyImage relationship
            builder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure RentalApplication - Property relationship
            builder.Entity<RentalApplication>()
                .HasOne(ra => ra.Property)
                .WithMany(p => p.Applications)
                .HasForeignKey(ra => ra.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure RentalApplication - Tenant relationship
            builder.Entity<RentalApplication>()
                .HasOne(ra => ra.Tenant)
                .WithMany(u => u.RentalApplications)
                .HasForeignKey(ra => ra.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal Precision
            builder.Entity<Property>()
                .Property(p => p.MonthlyRent)
                .HasPrecision(18, 2);

            builder.Entity<Property>()
                .Property(p => p.SecurityDeposit)
                .HasPrecision(18, 2);

            builder.Entity<RentalApplication>()
                .Property(ra => ra.MonthlyIncome)
                .HasPrecision(18, 2);

            // Indexes for search performance
            builder.Entity<Property>()
                .HasIndex(p => p.City);

            builder.Entity<Property>()
                .HasIndex(p => p.MonthlyRent);

            builder.Entity<Property>()
                .HasIndex(p => new { p.IsAvailable, p.IsActive });

            builder.Entity<RentalApplication>()
                .HasIndex(ra => new { ra.PropertyId, ra.TenantId, ra.Status });
        }
    }
}

