using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Geofence
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Column(TypeName = "float")]  // Use float without precision
    public double CenterLatitude { get; set; }

    [Required]
    [Column(TypeName = "float")]  // Use float without precision
    public double CenterLongitude { get; set; }

    [Required]
    [Column(TypeName = "float")]  // Use float without precision
    public double Radius { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}