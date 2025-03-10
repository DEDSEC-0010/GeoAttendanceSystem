public class AttendanceViewModel
{
    public float EmployeeId { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class AttendanceRecordViewModel
{
    public DateTime Timestamp { get; set; }
    public string LocationName { get; set; }
    public string Status { get; set; }
}