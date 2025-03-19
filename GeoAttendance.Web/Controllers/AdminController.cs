using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeoAttendance.Web.Models;
using System.Text;

namespace GeoAttendance.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Reports()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateReport()
        {
            try
            {
                // Create a dummy report (CSV format)
                var sb = new StringBuilder();

                // Add headers
                sb.AppendLine("Employee ID,Name,Date,Check In Time,Check Out Time,Status");

                // Add dummy data
                sb.AppendLine("1001,John Doe,2025-03-19,09:00 AM,05:30 PM,Present");
                sb.AppendLine("1002,Jane Smith,2025-03-19,09:15 AM,05:45 PM,Present");
                sb.AppendLine("1003,Mike Johnson,2025-03-19,09:30 AM,06:00 PM,Present");
                sb.AppendLine("1004,Sarah Williams,2025-03-19,,Absent");

                // Convert to bytes
                byte[] reportBytes = Encoding.UTF8.GetBytes(sb.ToString());

                // Return the file
                return File(reportBytes, "text/csv", $"attendance_report_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating attendance report");
                TempData["Error"] = "Could not generate report. Please try again.";
                return RedirectToAction(nameof(Reports));
            }
        }
    }
}