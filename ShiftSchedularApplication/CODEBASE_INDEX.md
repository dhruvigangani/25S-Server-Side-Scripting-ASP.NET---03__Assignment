# ShiftSchedularApplication - Codebase Index

## Project Overview
A .NET 8.0 ASP.NET Core MVC application for managing employee shift scheduling, time tracking, availability, and payroll. The application uses Entity Framework Core with SQL Server and includes Google OAuth authentication.

## Technology Stack
- **Framework**: .NET 8.0
- **Architecture**: ASP.NET Core MVC
- **Database**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core 9.0.7
- **Authentication**: ASP.NET Core Identity + Google OAuth
- **UI**: Bootstrap 5, jQuery
- **Development**: Visual Studio Web Code Generation

## Project Structure

### Core Application Files
- `Program.cs` - Application entry point and configuration
- `ShiftSchedularApplication.csproj` - Project file with dependencies
- `appsettings.json` - Configuration including database connection and Google OAuth

### Data Layer
- `Data/ApplicationDbContext.cs` - Entity Framework DbContext
- `Data/Migrations/` - Database migration files (9 migrations total)

### Models (Domain Entities)
- `Models/Shift.cs` - Employee work shifts with start/end times and status flags
- `Models/Availability.cs` - Employee availability by day of week and time ranges
- `Models/PayStub.cs` - Payroll records with hours worked and hourly rates
- `Models/Punch.cs` - Time clock punch in/out records
- `Models/ErrorViewModel.cs` - Error handling model

### Controllers (Business Logic)
- `Controllers/HomeController.cs` - Landing page and error handling
- `Controllers/ShiftsController.cs` - CRUD operations for work shifts
- `Controllers/PunchesController.cs` - CRUD operations for time clock punches
- `Controllers/AvailabilitiesController.cs` - CRUD operations for employee availability
- `Controllers/PayStubsController.cs` - CRUD operations for payroll records

### Views (Presentation Layer)
- `Views/Shared/_Layout.cshtml` - Main layout template with navigation
- `Views/Home/` - Landing page views
- `Views/Shifts/` - Shift management views (Index, Create, Edit, Delete, Details)
- `Views/Punches/` - Time tracking views (Index, Create, Edit, Delete, Details)
- `Views/Availabilities/` - Availability management views (Index, Create, Edit, Delete, Details)
- `Views/PayStubs/` - Payroll views (Index, Create, Edit, Delete, Details)

### Static Assets
- `wwwroot/css/site.css` - Custom styles
- `wwwroot/js/site.js` - Custom JavaScript
- `wwwroot/lib/` - Third-party libraries (Bootstrap, jQuery, validation)

## Database Schema

### Core Tables
1. **AspNetUsers** (Identity) - User accounts
2. **Shifts** - Work shift assignments
   - EmployeeId (FK to AspNetUsers)
   - StartTime, EndTime
   - IsSwapRequested, IsGivenAway, IsAbsent flags
3. **Punches** - Time clock records
   - EmployeeId (FK to AspNetUsers)
   - PunchInTime, PunchOutTime (nullable)
4. **Availabilities** - Employee availability
   - EmployeeId (FK to AspNetUsers)
   - Day (DayOfWeek enum)
   - StartAvailability, EndAvailability (TimeSpan)
5. **PayStubs** - Payroll records
   - EmployeeId (FK to AspNetUsers)
   - HoursWorked, HourlyRate, TotalPay (calculated)
   - PayDate

## Authentication & Authorization
- **Default Identity**: ASP.NET Core Identity with custom user seeding
- **Google OAuth**: External authentication provider
- **Authorization**: Mix of [Authorize] and [AllowAnonymous] attributes
- **Seeded User**: "dario@gc.ca" with password "Test123$"

## Key Features

### 1. Shift Management
- Create, read, update, delete work shifts
- Track shift status (swap requested, given away, absent)
- Associate shifts with employees via EmployeeId

### 2. Time Tracking
- Punch in/out functionality
- Track work hours with start and end times
- Support for incomplete punch records (PunchOutTime nullable)

### 3. Availability Management
- Set availability by day of week
- Time range specification (start/end availability)
- Employee-specific availability tracking

### 4. Payroll Processing
- Calculate pay based on hours worked and hourly rate
- Automatic total pay calculation
- Pay date tracking

### 5. User Management
- Google OAuth integration
- ASP.NET Core Identity for user accounts
- Role-based access control (basic implementation)

## Security Features
- CSRF protection with [ValidateAntiForgeryToken]
- Input validation with data annotations
- SQL injection prevention via Entity Framework
- Authentication required for most operations
- Anonymous access for viewing data (Index/Details actions)

## Development Setup
- **Database**: LocalDB with connection string in appsettings.json
- **Google OAuth**: Configured with client ID and secret
- **Dependencies**: Managed via NuGet packages
- **Migrations**: 9 migration files for database schema evolution

## Navigation Structure
- Home (landing page)
- Shifts (shift management)
- Punches (time tracking)
- PayStubs (payroll)
- Availability (employee availability)
- Login/Logout (authentication)

## Code Quality
- **Architecture**: Clean MVC separation
- **Validation**: Data annotations and model validation
- **Error Handling**: Try-catch blocks and proper error responses
- **Async/Await**: Proper asynchronous operations
- **Dependency Injection**: Constructor injection for DbContext

## Areas for Enhancement
1. Role-based authorization (Admin/Manager/Employee roles)
2. Shift conflict detection
3. Automated payroll calculations
4. Email notifications
5. Mobile-responsive design improvements
6. API endpoints for external integrations
7. Audit logging
8. Data export functionality

## Migration History
- Initial Identity schema creation
- Core entity tables (Shifts, Availabilities, PayStubs)
- Punch model additions and refinements
- Database constraint and precision fixes
- Final schema stabilization

This codebase represents a functional shift scheduling application with core business logic for managing employee schedules, time tracking, and payroll processing. 