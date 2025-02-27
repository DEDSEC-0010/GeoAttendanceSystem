using Microsoft.AspNetCore.Mvc;
using GeoAttendance.API.DTOs;
using GeoAttendance.Common.Models;
using GeoAttendance.API.Data;

namespace GeoAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AttendanceController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceRecord>> PostAttendance([FromBody] AttendanceRequest request)
    {
        var employee = await _context.Employees.FindAsync(request.EmployeeId);
        if (employee == null) return NotFound();

        var companyLat = _config.GetValue<double>("Geofence:Latitude");
        var companyLon = _config.GetValue<double>("Geofence:Longitude");
        var radius = _config.GetValue<double>("Geofence:Radius");

        var distance = CalculateDistance(companyLat, companyLon, request.Latitude, request.Longitude);

        var record = new AttendanceRecord
        {
            EmployeeId = request.EmployeeId,
            TimeStamp = DateTime.UtcNow,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsPresent = distance <= radius
        };

        _context.AttendanceRecords.Add(record);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(PostAttendance), record);
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371e3; // Earth radius in meters
        var φ1 = lat1 * Math.PI / 180;
        var φ2 = lat2 * Math.PI / 180;
        var Δφ = (lat2 - lat1) * Math.PI / 180;
        var Δλ = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                Math.Cos(φ1) * Math.Cos(φ2) *
                Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }
}