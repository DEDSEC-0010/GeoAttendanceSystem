using GeoAttendance.API.Data;
using GeoAttendance.API.DTOs;
using GeoAttendance.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoAttendance.API.Services
{
    public class AttendanceService
    {
        private readonly AppDbContext _context;
        private readonly GeofenceService _geofenceService;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(
            AppDbContext context,
            GeofenceService geofenceService,
            ILogger<AttendanceService> logger)
        {
            _context = context;
            _geofenceService = geofenceService;
            _logger = logger;
        }

        public async Task<(AttendanceRecord Record, bool Success, string Message)> MarkAttendanceAsync(AttendanceRequestDTO request)
        {
            try
            {
                // Validate coordinates
                if (request.Latitude < -90m || request.Latitude > 90m ||
                    request.Longitude < -180m || request.Longitude > 180m)
                {
                    return (null, false, "Invalid coordinates provided.");
                }

                // Check if user exists
                var employee = await _context.Employees
    .FirstOrDefaultAsync(e => e.Id.ToString() == request.EmployeeId);

                if (employee == null)
                {
                    return (null, false, "Employee not found.");
                }

                // Check if already marked attendance within last hour
                var lastAttendance = await _context.AttendanceRecords
                    .Where(a => a.EmployeeId == request.EmployeeId)
                    .OrderByDescending(a => a.Timestamp)
                    .FirstOrDefaultAsync();

                if (lastAttendance != null &&
                    (DateTime.UtcNow - lastAttendance.Timestamp).TotalHours < 1)
                {
                    return (null, false, "Attendance already marked within the last hour.");
                }

                // Check if location is within any active geofence
                var isWithinGeofence = await _geofenceService
                    .IsLocationWithinAnyGeofenceAsync(request.Latitude, request.Longitude);

                var record = new AttendanceRecord
                {
                    EmployeeId = request.EmployeeId,
                    Timestamp = DateTime.UtcNow,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    IsPresent = isWithinGeofence,
                    DeviceId = request.DeviceId
                };

                _context.AttendanceRecords.Add(record);
                await _context.SaveChangesAsync();

                var message = isWithinGeofence
                    ? "Attendance marked successfully. Status: Present"
                    : "Attendance marked successfully. Status: Absent (Outside office area)";

                return (record, true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking attendance for employee {EmployeeId}",
                    request.EmployeeId);
                return (null, false, "An error occurred while marking attendance.");
            }
        }

        public async Task<List<AttendanceRecord>> GetAttendanceHistoryAsync(
            string employeeId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var query = _context.AttendanceRecords
                    .Where(a => a.EmployeeId == employeeId);

                if (startDate.HasValue)
                    query = query.Where(a => a.Timestamp >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(a => a.Timestamp <= endDate.Value);

                return await query
                    .OrderByDescending(a => a.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance history for employee {EmployeeId}",
                    employeeId);
                throw;
            }
        }

        public async Task<AttendanceRecord> GetAttendanceByIdAsync(int id)
        {
            return await _context.AttendanceRecords.FindAsync(id);
        }
    }
}