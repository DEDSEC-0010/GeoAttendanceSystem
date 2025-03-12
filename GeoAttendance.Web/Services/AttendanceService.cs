using System.Net.Http.Json;
using GeoAttendance.Web.Models;
using System.Text.Json;

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
                // Make sure the request matches what the server expects
                var requestData = new
                {
                    EmployeeId = model.EmployeeId,  // Include the EmployeeId
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    DeviceId = model.DeviceId
                };

                var response = await _httpClient.PostAsJsonAsync("api/attendance", requestData);

                // First read the raw content
                var rawContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response: {Content}", rawContent);

                if (string.IsNullOrEmpty(rawContent))
                {
                    _logger.LogWarning("Received empty response from server");
                    return (false, "Inside geofence Cognizant F3");
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Server returned status code: {StatusCode}", response.StatusCode);
                    return (false, rawContent ?? $"Server returned status code: {response.StatusCode}");
                }

                try
                {
                    var attendanceResponse = await response.Content.ReadFromJsonAsync<AttendanceResponseDTO>();
                    if (attendanceResponse == null)
                    {
                        return (false, "Unable to parse server response");
                    }

                    return (true, attendanceResponse.Message ??
                           (attendanceResponse.IsPresent ? "Successfully marked as present" : "Marked as absent"));
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse JSON response: {Content}", rawContent);
                    return (false, "Invalid response format from server");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while marking attendance");
                return (false, "Failed to connect to the attendance service");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking attendance");
                return (false, "An unexpected error occurred while marking attendance");
            }
        }

        // Add this DTO class to match the server's response
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