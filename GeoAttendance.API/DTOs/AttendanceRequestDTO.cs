namespace GeoAttendance.API.DTOs
{
    public class AttendanceRequestDTO
    {
        public string EmployeeId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string DeviceId { get; set; }
    }

    public class AttendanceResponseDTO
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsPresent { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Message { get; set; }
    }
}