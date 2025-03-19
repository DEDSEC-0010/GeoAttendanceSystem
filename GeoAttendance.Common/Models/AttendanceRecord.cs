using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoAttendance.Common.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,8)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,8)")]
        public decimal Longitude { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        public string DeviceId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}