using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeoAttendance.Web.Services;
using GeoAttendance.Web.Models;

namespace GeoAttendance.Web.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IGeofenceService _geofenceService;
        private readonly IAttendanceService _attendanceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(
            IGeofenceService geofenceService,
            IAttendanceService attendanceService,
            ILogger<AttendanceController> logger)
        {
            _geofenceService = geofenceService;
            _attendanceService = attendanceService;
            _logger = logger;
        }

        public IActionResult MarkAttendance()
        {
            // Initialize a new view model with current timestamp
            var model = new AttendanceViewModel
            {
                Timestamp = DateTime.UtcNow
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttendance(AttendanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Ensure timestamp is set
                model.Timestamp = DateTime.UtcNow;

                var isWithinGeofence = await _geofenceService.CheckLocationAsync(
                    model.Latitude,
                    model.Longitude);

                if (!isWithinGeofence)
                {
                    ModelState.AddModelError("", "You are not within any approved office location.");
                    return View(model);
                }

                var result = await _attendanceService.MarkAttendanceAsync(model);
                if (result)
                {
                    TempData["Success"] = "Attendance marked successfully!";
                    return RedirectToAction(nameof(MyHistory));
                }

                ModelState.AddModelError("", "Failed to mark attendance. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking attendance");
                ModelState.AddModelError("", "An error occurred. Please try again.");
                return View(model);
            }
        }

        public async Task<IActionResult> MyHistory()
        {
            try
            {
                var history = await _attendanceService.GetUserAttendanceHistoryAsync();
                return View(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching attendance history");
                TempData["Error"] = "Could not load attendance history.";
                return View(Enumerable.Empty<AttendanceRecordViewModel>());
            }
        }
    }
}