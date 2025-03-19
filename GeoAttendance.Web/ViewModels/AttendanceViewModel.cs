using System.ComponentModel.DataAnnotations;

namespace GeoAttendance.Web.Models
{
    public class AttendanceViewModel
    {
        [Required]
        [Range(-90, 90)]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public decimal Longitude { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string DeviceId { get; set; }
    }

    public class AttendanceRecordViewModel
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsPresent { get; set; }
        public string LocationName { get; set; }
        public string Status => IsPresent ? "Present" : "Absent";
    }
}