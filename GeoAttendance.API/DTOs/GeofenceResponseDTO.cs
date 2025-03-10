public class GeofenceResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public double Radius { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}