using System.ComponentModel.DataAnnotations;

public class GeofenceViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Geofence Name")]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Latitude")]
    [Range(-90, 90)]
    public double CenterLatitude { get; set; }  // Changed from float to double

    [Required]
    [Display(Name = "Longitude")]
    [Range(-180, 180)]
    public double CenterLongitude { get; set; }  // Changed from float to double

    [Required]
    [Range(1, 10000)]
    [Display(Name = "Radius (meters)")]
    public double Radius { get; set; }  // Changed from float to double

    [StringLength(500)]
    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}