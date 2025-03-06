namespace GeoAttendance.API.DTOs;

public class GeofenceDTO
{
    public string Name { get; set; }
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public double Radius { get; set; }
    public string Description { get; set; }
}

public class UpdateGeofenceDTO : GeofenceDTO
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}

public class GeofenceResponseDTO : UpdateGeofenceDTO
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GeofenceLocationCheckDTO
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}