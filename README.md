# HOUSE RENTAL MANAGEMENT SYSTEM (RentHaven)

> **Major Academic Project for B.Tech in Computer Science & Engineering (AIML)**  
> Built with **ASP.NET Core 8 MVC**, C#, Entity Framework Core, SQL Server, ASP.NET Core Identity, and Bootstrap 5.

---

## 1. Project Introduction

The **House Rental Management System** (**RentHaven**) is an enterprise-grade web application engineered to bridge the gap between property owners (landlords) and prospective tenants. In urban centers and university towns, discovering reliable rental accommodations is traditionally fraught with high broker commissions, inaccurate property descriptions, and cumbersome paper application procedures.

RentHaven modernizes this process by providing a centralized, secure, transparent, and aesthetically refined platform where:
- **Tenants** can search and filter verified residential properties, inspect high-resolution imagery and amenities, submit online rental applications with financial profiles, and track application statuses in real-time.
- **Property Owners** can list, edit, and manage rental homes, upload property photos, toggle property availability, and review, approve, or reject tenant applications with custom remarks.
- **System Administrators** hold supervisory control through a dedicated analytics dashboard to moderate listings, deactivate inappropriate content, audit registered user accounts, and monitor rental metrics.

---

## 2. Project Objectives

1. **Eliminate Intermediaries & Brokerage Fees**: Direct tenant-to-owner connection reducing financial burden.
2. **Streamlined Rental Applications**: Standardized online applications detailing verified income, occupation, and desired move-in dates without duplicate active submissions.
3. **Robust Security & Role-Based Authorization**: Distinct access controls separating **Admin**, **PropertyOwner**, and **Tenant** privileges.
4. **Light Modern UI/UX Experience**: A clean, light-themed visual design with soft royal blue, beige, and emerald green accents, zero broken links, smooth animations, and toast notifications.
5. **Architectural Cleanliness**: Strict adherence to the Model-View-Controller (MVC) architectural pattern, separation of concerns, asynchronous database access, and ViewModel encapsulation.

---

## 3. Technology Stack

| Layer | Technology / Tool | Version | Purpose |
|---|---|---|---|
| **Web Framework** | ASP.NET Core MVC | .NET 8.0 | High-performance MVC framework |
| **Language** | C# | 12.0 | Strongly typed backend programming |
| **ORM** | Entity Framework Core | 8.0.12 | Code-First database mapping, LINQ queries, and migrations |
| **Database** | Microsoft SQL Server / SQLite | 2019/2022 / 3.x | Relational storage for users, listings, and applications |
| **Authentication** | ASP.NET Core Identity | 8.0.12 | PBKDF2 password hashing, cookie authentication, RBAC |
| **Frontend Styling** | Bootstrap 5, Bootstrap Icons | 5.3.3 / 1.11.3 | Responsive grid, interactive modals, toasts, and components |
| **Typography** | Plus Jakarta Sans | Google Fonts | Modern geometric sans-serif typography |
| **IDE / Environment** | Google Antigravity / Visual Studio / VS Code | Latest | Development and debugging |

---

## 4. Key Functional Modules

### Module 1: Modern Light-Themed Home Page
- **Hero Section**: Integrated search form allowing immediate filtering by city, property type, bedrooms, and budget.
- **Property Categories**: Filter by Apartments, Luxury Villas, Independent Homes, Studios, and Modern Condos.
- **Featured Listings**: Curated listings with pricing badges, room specifications, and availability indicators.
- **Popular Destinations**: High-resolution destination cards highlighting top rental cities.
- **How It Works**: Visual three-step workflow explaining search, application, and move-in procedures.
- **Direct Concierge Form**: Contact section with automated confirmation toasts.

### Module 2: Authentication & Role-Based Authorization
- **Registration**: Custom registration allowing users to self-register as either **Tenant** or **PropertyOwner**.
- **Login & Logout**: Secure cookie-based authentication with "Remember Me" capability and redirection.
- **Three Distinct Roles**:
  - `Admin`: Global oversight, account activation/deactivation, listing moderation.
  - `PropertyOwner`: Listing CRUD, availability toggle, tenant application review.
  - `Tenant`: Search, application submission, status tracking, application cancellation.

### Module 3: Property Listing Management
- **Property Creation**: Secure image upload validation (JPG, JPEG, PNG, WEBP up to 5MB) with unique GUID filenames.
- **Attribute Fields**: Title, description, address, city, state, zip code, monthly rent, security deposit, bedrooms, bathrooms, area (sq ft), furnishing status, and amenities checkboxes.
- **Ownership Verification**: Property edits and deletions are validated on the server to prevent unauthorized modifications.
- **Availability Toggle**: Landlords can mark properties as "Available" or "Rented / Unavailable" with one click.

### Module 4: Intelligent Property Search & Discovery
- **Multi-parameter Filtering**: Filter concurrently by keyword, city dropdown, property type, bedroom count, and min/max rent brackets.
- **Dynamic Sorting**: Sort by Newest, Rent: Low to High, Rent: High to Low, or Largest Area (Sq Ft).
- **Responsive Pagination**: Clean page navigation with retained filter states.
- **Property Details View**: Photo gallery, feature matrix, amenity checklists, landlord contact bio, and similar property recommendations.

### Module 5: Tenant Dashboard
- **Application Tracking**: Monitor application states: `Pending`, `Approved`, `Rejected`, or `Cancelled`.
- **Duplicate Prevention**: Prevents tenants from submitting duplicate active applications for the same property.
- **Self-Service Cancellation**: Tenants can cancel pending applications at any time.
- **Profile Management**: Update full name, phone number, and residential address.

### Module 6: Landlord Application Review System
- **Application Inbox**: Filter submissions by specific property and application status.
- **Applicant Dossier**: Inspect verified monthly income, occupation/employer, occupant count, preferred move-in date, and personal message.
- **Status Decisioning**: Approve or reject with customized feedback remarks displayed directly to the tenant.

### Module 7: Administrative Oversight Console
- **Platform Analytics**: Total users, owners, tenants, listed properties, available homes, and pending applications.
- **Listing Moderation**: Instantly deactivate listings that violate platform standards.
- **User Moderation**: Lock/unlock or deactivate suspicious accounts (with protection against self-deactivation for admins).
- **Audit Trails**: Full system visibility into every rental application submitted.

---

## 5. Project Directory Structure

```text
House Rental Management System/
│
├── Controllers/
│   ├── AccountController.cs          # Register, Login, Logout, Profile, RBAC
│   ├── AdminController.cs            # Analytics, User & Listing Moderation
│   ├── ApplicationsController.cs     # Tenant Apply, Cancel, Landlord Review
│   ├── HomeController.cs             # Landing page, Concierge message, Privacy
│   └── PropertiesController.cs       # Search, Filter, Sort, Property CRUD
│
├── Data/
│   ├── ApplicationDbContext.cs       # EF Core mappings, foreign keys, cascades, indexes
│   └── DbInitializer.cs              # Database seeding with default roles, demo accounts, properties
│
├── Migrations/
│   ├── 20261026182846_InitialCreate.cs # Initial database schema migration
│   └── ApplicationDbContextModelSnapshot.cs
│
├── Models/
│   ├── ApplicationUser.cs            # Extends IdentityUser with FullName, Role, Address
│   ├── Property.cs                   # Rental property entity and data validation
│   ├── PropertyImage.cs              # Gallery photo relationships
│   ├── RentalApplication.cs          # Application entity linking Tenant to Property
│   └── Enums/
│       └── ApplicationStatus.cs      # Pending, Approved, Rejected, Cancelled
│
├── Services/
│   ├── IFileStorageService.cs        # Secure image file upload contract
│   └── FileStorageService.cs         # Implementation with extension & size validation
│
├── ViewModels/
│   ├── Account/                      # RegisterViewModel, LoginViewModel, ProfileViewModel
│   ├── Application/                  # RentalApplicationCreateViewModel, StatusUpdateViewModel
│   ├── Dashboard/                    # AdminDashboardViewModel, TenantDashboardViewModel
│   ├── Home/                         # HomeViewModel, LocationStatItem, CategoryStatItem
│   └── Property/                     # PropertyCreateEditViewModel, PropertySearchViewModel
│
├── Views/
│   ├── Account/                      # Login, Register, Profile, AccessDenied
│   ├── Admin/                        # Index (Dashboard), Users, Properties, Applications
│   ├── Applications/                 # Apply, MyApplications, OwnerApplications, Details
│   ├── Home/                         # Index (Landing Page), Privacy
│   ├── Properties/                   # Index (Search), Details, Create, Edit, MyProperties
│   └── Shared/
│       ├── _Layout.cshtml            # Master light-themed responsive layout with Toasts
│       ├── _PropertyCard.cshtml      # Reusable property card partial view
│       └── _ValidationScriptsPartial.cshtml
│
├── wwwroot/
│   ├── css/site.css                  # Custom Light Theme stylesheet
│   ├── js/site.js                    # Client-side scripts
│   └── uploads/properties/           # User-uploaded property photos
│
├── appsettings.json                  # Production configuration & SQL Server connection string
├── appsettings.Development.json      # Local development configuration (SQLite fallback enabled)
├── Program.cs                        # DI setup, Identity rules, middleware, auto-seeder
└── HouseRentalManagementSystem.csproj
```

---

## 6. Installation & Execution Guide

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, SQL Express, or standard SQL Server) OR SQLite (included out-of-the-box)

### Step 1: Clone or Navigate to the Workspace
```powershell
cd "c:\Users\MANAN\Downloads\House Rental Management System"
```

### Step 2: Configure Database Connection
Open `appsettings.json`. By default, it targets SQL Server:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HouseRentalDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "DatabaseProvider": "SqlServer"
}
```
> **Note**: If you are presenting or testing on a computer without a local SQL Server instance, simply set `"DatabaseProvider": "Sqlite"` in `appsettings.json` (or use the included `appsettings.Development.json`). Both engines are natively supported!

### Step 3: Run Database Migrations
To apply the generated migrations to your SQL Server database:
```powershell
dotnet ef database update
```
*(The application also automatically calls `context.Database.EnsureCreatedAsync()` and `DbInitializer.SeedAsync()` on first run).*

### Step 4: Build and Run
```powershell
dotnet run --launch-profile http
```
The application will start listening at:
**`http://localhost:5200`**

---

## 7. Pre-Seeded Demo User Accounts

Upon startup, the system automatically provisions three demo accounts with diverse roles for testing and evaluation:

| Role | Email Address | Password | Permissions & Dashboard |
|---|---|---|---|
| **Administrator** | `admin@houserental.com` | `Admin@123` | Full Admin Console, User & Property moderation |
| **Property Owner** | `owner@houserental.com` | `Owner@123` | List properties, edit/delete, review applications |
| **Tenant** | `tenant@houserental.com` | `Tenant@123` | Browse properties, apply online, track status |

---

## 8. College Project Summary (For Submission & Project Report)

### Abstract
The **House Rental Management System** is a full-stack web application developed to simplify residential property rentals. By leveraging **ASP.NET Core 8 MVC** and **Entity Framework Core**, the system provides a secure, role-based architecture serving three distinct personas: Tenants, Property Owners, and System Administrators. 

The application replaces informal brokerage channels with a verifiable digital workflow that handles property listing creation, image storage, multi-criteria property search, applicant vetting, and application status lifecycle management (`Pending` $\rightarrow$ `Approved` / `Rejected` / `Cancelled`). The system emphasizes user experience through a responsive Bootstrap 5 light theme, asynchronous database interactions, and strict validation against unauthorized data tampering.

---

## 9. Viva Voce Questions & Answers

### Q1: Why did you choose ASP.NET Core MVC for this project?
**Answer:** ASP.NET Core MVC is a cross-platform, high-performance framework that implements the Model-View-Controller design pattern. It provides native Dependency Injection, built-in security features (CSRF tokens, Identity, cookie authentication), and clean separation of concerns between business logic, presentation, and data access.

### Q2: How does the system handle security and prevent unauthorized edits to properties?
**Answer:** We enforce multi-layer security:
1. **Role-Based Authorization**: Endpoints like `Create` and `Edit` are decorated with `[Authorize(Roles = "PropertyOwner,Admin")]`.
2. **Server-Side Ownership Validation**: Before saving any edit or delete operation, the controller checks whether `property.OwnerId == currentUserId` or `User.IsInRole("Admin")`. If the user is neither the owner nor an administrator, access is denied.
3. **Anti-Forgery Tokens**: Every POST form includes `@Html.AntiForgeryToken()` to prevent Cross-Site Request Forgery (CSRF) attacks.
4. **File Upload Security**: Uploaded files are inspected for allowed file extensions (`.jpg`, `.jpeg`, `.png`, `.webp`), size capped at 5MB, and stored using generated GUID filenames to prevent path traversal attacks.

### Q3: How is database access handled, and how does the dual-provider setup work?
**Answer:** Database interactions are mediated by **Entity Framework Core** using the Code-First approach. In `Program.cs`, the DI container reads `DatabaseProvider` from configuration. If set to `"SqlServer"`, it configures `UseSqlServer()`. If set to `"Sqlite"`, it configures `UseSqlite()`. This flexibility ensures the project runs on both enterprise SQL Server environments and portable demonstration systems.

### Q4: How is duplicate application prevention enforced?
**Answer:** In `ApplicationsController.Apply`, before adding an application to the database, a query verifies whether an application with the same `PropertyId` and `TenantId` exists with status `Pending` or `Approved`. If an active application is found, the submission is rejected with a clear notification.

### Q5: What is the significance of ViewModels in your project?
**Answer:** ViewModels decouple database domain entities from UI presentation. For example, `PropertyCreateEditViewModel` encapsulates `IFormFile` upload objects and checkboxes for amenities, which do not directly exist as database columns, keeping the `Property` domain entity clean and preventing over-posting vulnerabilities.

---

## 10. Future Enhancements

1. **AI-Powered Rent Estimation & Recommendation Engine**: Using machine learning regression models to suggest competitive rental pricing based on location, area, and amenities.
2. **Integrated Payment Gateway**: Integration with Stripe or Razorpay for instant security deposit and monthly rent transactions.
3. **Real-Time Landlord-Tenant Chat**: WebSockets / SignalR implementation for instant messaging and lease agreement negotiations.
4. **Digital Lease Agreement Signing**: Integrated e-signature capability for legal tenancy agreements.

