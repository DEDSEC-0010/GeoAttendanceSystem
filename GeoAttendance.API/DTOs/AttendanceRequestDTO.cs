namespace GeoAttendance.API.DTOs;

public class AttendanceRequestDTO
{
    public string DeviceId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}