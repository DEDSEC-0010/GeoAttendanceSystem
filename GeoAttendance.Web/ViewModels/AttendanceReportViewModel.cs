using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeoAttendance.Web.Models
{
    public class AttendanceReportViewModel
    {
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        public List<AttendanceReportItemViewModel> AttendanceRecords { get; set; } = new();

        public List<SelectListItem> Employees { get; set; } = new();
    }

    public class AttendanceReportItemViewModel
    {
        public string EmployeeName { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsPresent { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}