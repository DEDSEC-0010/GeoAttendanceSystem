using System.Text;
using GeoAttendance.Web.Models;

namespace GeoAttendance.Web.Services
{
    public class ReportsService : IReportsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReportsService> _logger;

        public ReportsService(HttpClient httpClient, ILogger<ReportsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<AttendanceReportItemViewModel>> GetAttendanceReportAsync(
            DateTime startDate,
            DateTime endDate,
            int? employeeId)
        {
            try
            {
                var url = $"api/attendance/report?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
                if (employeeId.HasValue)
                {
                    url += $"&employeeId={employeeId.Value}";
                }

                var response = await _httpClient.GetFromJsonAsync<List<AttendanceReportItemViewModel>>(url);
                return response ?? new List<AttendanceReportItemViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance report");
                throw;
            }
        }

        public string GenerateCSV(List<AttendanceReportItemViewModel> records)
        {
            var csv = new StringBuilder();

            // Add headers
            csv.AppendLine("Employee Name,Date,Time,Status,Location");

            // Add records
            foreach (var record in records)
            {
                csv.AppendLine($"{record.EmployeeName}," +
                             $"{record.Timestamp:yyyy-MM-dd}," +
                             $"{record.Timestamp:HH:mm:ss}," +
                             $"{(record.IsPresent ? "Present" : "Absent")}," +
                             $"\"{record.Latitude},{record.Longitude}\"");
            }

            return csv.ToString();
        }
    }
}