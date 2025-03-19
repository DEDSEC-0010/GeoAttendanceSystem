using GeoAttendance.Web.Models;

namespace GeoAttendance.Web.Services
{
    public interface IReportsService
    {
        Task<List<AttendanceReportItemViewModel>> GetAttendanceReportAsync(DateTime startDate, DateTime endDate, int? employeeId);
        string GenerateCSV(List<AttendanceReportItemViewModel> records);
    }
}