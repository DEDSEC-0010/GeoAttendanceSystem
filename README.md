# GeoAttendance System

A modern, web-based attendance management system that uses geolocation to track and verify employee attendance through geofencing technology. Built with C#, ASP.NET Core, and modern web technologies.

## 🌟 Features

- **Geofence-Based Attendance**
  - Real-time location tracking
  - Secure perimeter boundaries
  - Automated presence verification

- **User Authentication & Authorization**
  - Role-based access control (Admin/Employee)
  - Secure JWT-based API authentication and cookie-based web application authentication
  - 60-minute session duration

- **Comprehensive Attendance Management**
  - Real-time attendance marking
  - Attendance history tracking
  - Attendance timestamping allowing for punctuality analysis

- **Geofence Management (Admin)**
  - Create, Read, Update, and Delete (CRUD) geofences.
  - Visually manage geofences on a map.

- **Analytics & Reporting**
  - Comprehensive attendance reports
  - Filter reports by date range and employee.
  - CSV export functionality for attendance reports.

- **User Management (Admin)**
  - List and manage employee user accounts.

- **User Feedback and Alerts**
  - Attendance confirmation alerts
  - Geofence boundary alerts (prevents out-of-bound actions)

## 🛠️ Tech Stack

- **Backend**: C# (58.1%)
  - ASP.NET Core
  - Entity Framework Core
  - NetTopologySuite for geospatial operations
  - ASP.NET Core Identity for user management and JWT for secure API authentication.
  - Swagger/OpenAPI for API documentation.

- **Frontend**: 
  - HTML (32.6%)
  - CSS (6.8%)
  - JavaScript (2.5%)
  - Bootstrap for responsive design

- **Security**:
  - Cookie-based authentication for the Web application.

- **Features**:
  - Google Maps API integration
  - Real-time geolocation tracking
  - Secure API communication
  - Responsive web interface

## 📋 Prerequisites

- net9.0 SDK or later
- SQL Server (for database)
- Google Maps API key
- Modern web browser with geolocation support

## 🚀 Getting Started

1. Clone the repository
```bash
git clone https://github.com/DEDSEC-0010/GeoAttendanceSystem.git
cd GeoAttendanceSystem
```

2. Configure the application
   1.  **API Configuration (`GeoAttendance.API/appsettings.json`):**
       -   Update `ConnectionStrings:DefaultConnection` with your SQL Server details.
       -   Review and update `Jwt:Key` with a strong, unique secret. Ensure this key matches the one in the Web application's settings.
   2.  **Web Application Configuration (`GeoAttendance.Web/appsettings.json`):**
       -   Update `GoogleMaps:ApiKey` with your Google Maps API key.
       -   Verify `ApiSettings:BaseUrl` matches the running URL of the GeoAttendance.API (e.g., `http://localhost:5201`).
       -   Ensure `Jwt:Key` matches the one in the API application's settings.
   3.  **Database Setup:**
       -   Ensure the database specified in the connection string exists or can be created by Entity Framework Core.
       -   Run database migrations. After building the solution, you can do this by running the `GeoAttendance.API` project. It's configured to apply migrations on startup. Alternatively, use the .NET CLI: `dotnet ef database update --project GeoAttendance.API`.
   4.  **Initial Geofence Setup:**
       -   Once the application is running, log in as an administrator.
       -   Navigate to the Geofence management section to define initial geofence boundaries.

3. Running the Application
   -   Run the `GeoAttendance.API` project.
   -   Run the `GeoAttendance.Web` project.
   -   Access the web application through its configured URL.
---
