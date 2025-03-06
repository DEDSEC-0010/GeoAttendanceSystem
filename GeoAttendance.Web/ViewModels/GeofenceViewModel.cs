using System.ComponentModel.DataAnnotations;

namespace GeoAttendance.Web.Models
{
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
        public float CenterLatitude { get; set; }

        [Required]
        [Display(Name = "Longitude")]
        [Range(-180, 180)]
        public float CenterLongitude { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Radius (meters)")]
        public float Radius { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}