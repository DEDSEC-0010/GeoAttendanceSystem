using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GeoAttendance.Web.Services;
using GeoAttendance.Web.Models;
using System;
using System.Threading.Tasks;

namespace GeoAttendance.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        public IActionResult Index()
        {
            var model = new AttendanceReportViewModel
            {
                StartDate = DateTime.UtcNow.Date.AddDays(-30),
                EndDate = DateTime.UtcNow.Date,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(AttendanceReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var report = await _reportsService.GetAttendanceReportAsync(
                    model.StartDate,
                    model.EndDate,
                    model.EmployeeId);

                model.AttendanceRecords = report;
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to generate report. Please try again.");
                return View(model);
            }
        }

        public async Task<IActionResult> Export(DateTime startDate, DateTime endDate, int? employeeId)
        {
            var report = await _reportsService.GetAttendanceReportAsync(startDate, endDate, employeeId);
            var csvContent = _reportsService.GenerateCSV(report);

            return File(new System.Text.UTF8Encoding().GetBytes(csvContent),
                "text/csv",
                $"attendance_report_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
    }
}