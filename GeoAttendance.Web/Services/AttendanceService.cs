using GeoAttendance.Web.Models;
using System.Net.Http.Json;

namespace GeoAttendance.Web.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AttendanceService> _logger;
        private readonly string _baseUrl;

        public AttendanceService(HttpClient httpClient, IConfiguration configuration, ILogger<AttendanceService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"] + "/api/attendance";
        }

        public async Task<bool> MarkAttendanceAsync(AttendanceViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/mark", model);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking attendance");
                throw;
            }
        }

        public async Task<IEnumerable<AttendanceRecordViewModel>> GetUserAttendanceHistoryAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<AttendanceRecordViewModel>>($"{_baseUrl}/history");
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