using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeoAttendance.API.DTOs;
using GeoAttendance.Common.Models;
using GeoAttendance.API.Data;
using GeoAttendance.API.Services;
using Microsoft.EntityFrameworkCore;

namespace GeoAttendance.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly GeofenceService _geofenceService;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(
        AppDbContext context,
        GeofenceService geofenceService,
        ILogger<AttendanceController> logger)
    {
        _context = context;
        _geofenceService = geofenceService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceResponseDTO>> PostAttendance([FromBody] AttendanceRequest request)
    {
        try
        {
            var employee = await _context.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
                return NotFound("Employee not found");

            // Check if location is within any active geofence
            var isWithinGeofence = await _geofenceService.IsLocationWithinAnyGeofenceAsync(
                (decimal)request.Latitude,
                (decimal)request.Longitude
            );

            var record = new AttendanceRecord
            {
                EmployeeId = request.EmployeeId,
                TimeStamp = DateTime.UtcNow,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsPresent = isWithinGeofence
            };

            _context.AttendanceRecords.Add(record);
            await _context.SaveChangesAsync();

            var response = new AttendanceResponseDTO
            {
                Id = record.Id,
                EmployeeId = record.EmployeeId,
                TimeStamp = record.TimeStamp,
                IsPresent = record.IsPresent,
                Location = new LocationDTO
                {
                    Latitude = record.Latitude,
                    Longitude = record.Longitude
                },
                Message = isWithinGeofence
                    ? "Attendance recorded successfully within designated area."
                    : "Attendance recorded but location is outside designated area."
            };

            return CreatedAtAction(nameof(GetAttendance), new { id = record.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording attendance for employee {EmployeeId}", request.EmployeeId);
            return StatusCode(500, "An error occurred while recording attendance");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AttendanceResponseDTO>> GetAttendance(int id)
    {
        var record = await _context.AttendanceRecords.FindAsync(id);
        if (record == null)
            return NotFound();

        return Ok(new AttendanceResponseDTO
        {
            Id = record.Id,
            EmployeeId = record.EmployeeId,
            TimeStamp = record.TimeStamp,
            IsPresent = record.IsPresent,
            Location = new LocationDTO
            {
                Latitude = record.Latitude,
                Longitude = record.Longitude
            }
        });
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<AttendanceResponseDTO>>> GetEmployeeAttendance(
        int employeeId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = _context.AttendanceRecords
            .Where(r => r.EmployeeId == employeeId);

        if (startDate.HasValue)
            query = query.Where(r => r.TimeStamp >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(r => r.TimeStamp <= endDate.Value);

        var records = await query
            .OrderByDescending(r => r.TimeStamp)
            .Take(100)  // Limit the number of records returned
            .ToListAsync();

        return Ok(records.Select(r => new AttendanceResponseDTO
        {
            Id = r.Id,
            EmployeeId = r.EmployeeId,
            TimeStamp = r.TimeStamp,
            IsPresent = r.IsPresent,
            Location = new LocationDTO
            {
                Latitude = r.Latitude,
                Longitude = r.Longitude
            }
        }));
    }
}

public class AttendanceResponseDTO
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime TimeStamp { get; set; }
    public bool IsPresent { get; set; }
    public LocationDTO Location { get; set; }
    public string Message { get; set; }
}

public class LocationDTO
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}