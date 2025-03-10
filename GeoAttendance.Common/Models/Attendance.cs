using NetTopologySuite.Geometries;

namespace GeoAttendance.Common.Models;

public class Attendance
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Point Location { get; set; }
    public bool IsPresent { get; set; }
    public string DeviceId { get; set; }
    public string UserId { get; set; }
}