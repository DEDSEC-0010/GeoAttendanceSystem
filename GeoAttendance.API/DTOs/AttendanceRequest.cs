namespace GeoAttendance.API.DTOs
{
    public class AttendanceRequest
    {
        public int EmployeeId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
