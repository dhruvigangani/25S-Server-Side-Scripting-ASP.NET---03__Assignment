# 🕐 ShiftScheduler Application

A comprehensive shift management system built with ASP.NET Core, designed to help businesses efficiently manage employee schedules, availability, time tracking, and payroll.

## ✨ Features

- **📅 Shift Management**: Create, edit, and manage employee shifts
- **👥 Availability Tracking**: Track employee availability and preferences
- **⏰ Time Tracking**: Punch in/out functionality for accurate time tracking
- **💰 Payroll Integration**: Generate pay stubs based on hours worked
- **🔐 User Authentication**: Secure login with Google OAuth support
- **📱 Responsive Design**: Works on desktop and mobile devices

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL Database
- Git

### Local Development
```bash
# Clone the repository
git clone https://github.com/dhruvigangani/25S-Server-Side-Scripting-ASP.NET---03__Assignment.git

# Navigate to the project
cd ShiftSchedularApplication

# Restore dependencies
dotnet restore

# Build the application
dotnet build

# Run the application
dotnet run
```

### Environment Setup
1. Set up your database connection string in `appsettings.json`
2. Configure Google OAuth (optional) in `appsettings.json`
3. Run database migrations: `dotnet ef database update`

## 🏗️ Architecture

### Technology Stack
- **Backend**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with Google OAuth
- **Frontend**: Razor Pages with Bootstrap
- **Deployment**: Render.com

### Project Structure
```
ShiftSchedularApplication/
├── Controllers/          # MVC Controllers
├── Models/              # Data Models
├── Views/               # Razor Views
├── Data/                # Database Context
├── Areas/               # Identity Pages
└── wwwroot/            # Static Files
```

## 📋 Core Features

### 1. Shift Management
- Create and edit shifts
- Assign employees to shifts
- Track shift status (swap requested, given away, absent)
- View shift history

### 2. Availability Tracking
- Set employee availability by day
- Define start and end availability times
- Manage scheduling preferences

### 3. Time Tracking
- Punch in/out functionality
- Track actual work hours
- Generate time reports

### 4. Payroll System
- Automatic pay stub generation
- Calculate wages based on hours worked
- Track hourly rates and overtime

## 🔧 Configuration

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=shiftschedulerdb;Username=postgres;Password=password"
  }
}
```

### Google OAuth (Optional)
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

## 🚀 Deployment

### Deploy to Render
1. Fork or clone this repository
2. Create a Render account at [render.com](https://render.com)
3. Create a new Web Service
4. Connect your GitHub repository
5. Configure environment variables
6. Deploy!

For detailed deployment instructions, see [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)

## 📊 Database Schema

### Core Entities
- **Shifts**: Employee shift assignments and scheduling
- **Availabilities**: Employee availability preferences
- **Punches**: Time tracking records
- **PayStubs**: Payroll and compensation records
- **Users**: Identity and authentication

## 🔐 Security Features

- ASP.NET Core Identity for user management
- Google OAuth integration
- Secure password hashing
- HTTPS enforcement in production
- SQL injection protection via Entity Framework

## 🧪 Testing

```bash
# Run tests
dotnet test

# Build in Release mode
dotnet build --configuration Release
```

## 📝 API Endpoints

### Shifts
- `GET /Shifts` - List all shifts
- `POST /Shifts/Create` - Create new shift
- `PUT /Shifts/Edit/{id}` - Update shift
- `DELETE /Shifts/Delete/{id}` - Delete shift

### Availabilities
- `GET /Availabilities` - List availabilities
- `POST /Availabilities/Create` - Create availability
- `PUT /Availabilities/Edit/{id}` - Update availability

### Punches
- `GET /Punches` - List punch records
- `POST /Punches/Create` - Create punch record
- `PUT /Punches/Edit/{id}` - Update punch record

### PayStubs
- `GET /PayStubs` - List pay stubs
- `POST /PayStubs/Create` - Generate pay stub
- `GET /PayStubs/Details/{id}` - View pay stub details

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature-name`
3. Make your changes
4. Commit your changes: `git commit -m 'Add feature'`
5. Push to the branch: `git push origin feature-name`
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

If you encounter any issues:

1. Check the [deployment guide](DEPLOYMENT_GUIDE.md)
2. Review the [troubleshooting section](DEPLOYMENT_GUIDE.md#troubleshooting)
3. Open an issue on GitHub
4. Contact the development team

## 🎯 Roadmap

- [ ] Mobile app development
- [ ] Advanced reporting features
- [ ] Integration with payroll systems
- [ ] Real-time notifications
- [ ] API documentation
- [ ] Unit test coverage

---

**Built with ❤️ using ASP.NET Core**

*For detailed deployment instructions, see [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)* 