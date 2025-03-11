using System.Net.Http.Json;
using GeoAttendance.Web.Models;
using GeoAttendance.Web.Services;
namespace GeoAttendance.Web.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(
            HttpClient httpClient,
            ILogger<AttendanceService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> MarkAttendanceAsync(AttendanceViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/attendance/mark", new
                {
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    Timestamp = DateTime.UtcNow,
                    DeviceId = model.DeviceId
                });

                var content = await response.Content.ReadFromJsonAsync<dynamic>();
                string message = content?.message ?? "Unknown response from server";

                if (response.IsSuccessStatusCode)
                {
                    bool isPresent = content?.isPresent ?? false;
                    string status = isPresent ? "Present" : "Absent";
                    return (true, $"Attendance marked successfully. Status: {status}");
                }

                _logger.LogWarning("Failed to mark attendance: {Message}", message);
                return (false, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking attendance");
                return (false, "An error occurred while marking attendance.");
            }
        }

        public async Task<IEnumerable<AttendanceRecordViewModel>> GetUserAttendanceHistoryAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<AttendanceRecordViewModel>>(
                    "api/attendance/history");
                return response ?? Enumerable.Empty<AttendanceRecordViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance history");
                throw;
            }
        }

        
    }
}