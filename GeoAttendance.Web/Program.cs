using System.Net.Http.Headers;
using System.Security.Claims;
using ClosedXML.Excel;
using GeoAttendance.Web;
using GeoAttendance.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();


builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:5201/");
    client.DefaultRequestHeaders.Accept.Add(
       new MediaTypeWithQualityHeaderValue("application/json"));                                   // Match API's HTTPS port
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));

    options.AddPolicy("EmployeeOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Employee"));
});

builder.Services.AddHttpClient<IAttendanceService, AttendanceService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
});





builder.Services.AddHttpClient<IGeofenceService, GeofenceService>();
builder.Services.AddHttpClient<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IReportsService, ReportsService>();

// Build app
var app = builder.Build();
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();