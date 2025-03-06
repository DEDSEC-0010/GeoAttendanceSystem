using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoAttendance.Common.Models;

[Table("Geofences")]
public class Geofence
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("CenterLatitude", TypeName = "float")]  // Explicitly specify SQL Server type
    public double CenterLatitude { get; set; }

    [Column("CenterLongitude", TypeName = "float")]  // Explicitly specify SQL Server type
    public double CenterLongitude { get; set; }

    [Column(TypeName = "float")]  // Explicitly specify SQL Server type
    public double Radius { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
