// Add this model
public enum AttendanceType
{
    CheckIn,
    CheckOut
}

public class AttendanceRequest
{
    public int EmployeeId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public AttendanceType Type { get; set; }  // Add this
}