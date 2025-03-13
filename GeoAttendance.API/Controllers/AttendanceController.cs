using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeoAttendance.API.Services;
using GeoAttendance.API.DTOs;

namespace GeoAttendance.API.Controllers
{
   // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(
            AttendanceService attendanceService,
            ILogger<AttendanceController> logger)
        {
            _attendanceService = attendanceService;
            _logger = logger;
        }

        [HttpPost("mark")]
        public async Task<ActionResult<AttendanceResponseDTO>> MarkAttendance([FromBody] AttendanceRequestDTO request)
        {
            try
            {
                var (record, success, message) = await _attendanceService.MarkAttendanceAsync(request);

                if (!success)
                {
                    return BadRequest(new { message });
                }

                var response = new AttendanceResponseDTO
                {
                    Id = record.Id,
                    EmployeeId = record.EmployeeId,
                    Timestamp = record.Timestamp,
                    IsPresent = record.IsPresent,
                    Latitude = record.Latitude,
                    Longitude = record.Longitude,
                    Message = message
                };

                return CreatedAtAction(nameof(GetAttendance), new { id = record.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing attendance request");
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceResponseDTO>> GetAttendance(int id)
        {
            var record = await _attendanceService.GetAttendanceByIdAsync(id);

            if (record == null)
                return NotFound(new { message = "Attendance record not found." });

            return Ok(new AttendanceResponseDTO
            {
                Id = record.Id,
                EmployeeId = record.EmployeeId,
                Timestamp = record.Timestamp,
                IsPresent = record.IsPresent,
                Latitude = record.Latitude,
                Longitude = record.Longitude
            });
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<AttendanceResponseDTO>>> GetAttendanceHistory(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var employeeId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(employeeId))
                return Unauthorized(new { message = "Invalid user credentials." });

            var records = await _attendanceService.GetAttendanceHistoryAsync(employeeId, startDate, endDate);

            return Ok(records.Select(r => new AttendanceResponseDTO
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                Timestamp = r.Timestamp,
                IsPresent = r.IsPresent,
                Latitude = r.Latitude,
                Longitude = r.Longitude
            }));
        }
    }
}