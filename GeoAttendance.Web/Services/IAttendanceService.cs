using GeoAttendance.Web.Models;

namespace GeoAttendance.Web.Services
{
    public interface IAttendanceService
    {
        Task<(bool Success, string Message)> MarkAttendanceAsync(AttendanceViewModel model);
        Task<IEnumerable<AttendanceRecordViewModel>> GetUserAttendanceHistoryAsync();
    }
}