﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeoAttendance.Web.Models;
using GeoAttendance.Web.Services;

namespace GeoAttendance.Web.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IGeofenceService _geofenceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(
            IAttendanceService attendanceService,
            IGeofenceService geofenceService,
            ILogger<AttendanceController> logger)
        {
            _attendanceService = attendanceService;
            _geofenceService = geofenceService;
            _logger = logger;
        }

        public IActionResult MarkAttendance()
        {
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
                _logger.LogInformation(
                    "Attempting to mark attendance. Location: ({Latitude}, {Longitude})",
                    model.Latitude, model.Longitude);

                var isWithinGeofence = await _geofenceService.CheckLocationAsync(
                    (float)model.Latitude,
                    (float)model.Longitude);

                if (!isWithinGeofence)
                {
                    ModelState.AddModelError("", "You must be within an approved office location to mark attendance.");
                    return View(model);
                }

                var (success, message) = await _attendanceService.MarkAttendanceAsync(model);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(MyHistory));
                }

                ModelState.AddModelError("", message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking attendance");
                ModelState.AddModelError("", "An error occurred while marking attendance.");
                return View(model);
            }
        }

        public async Task<IActionResult> MyHistory()
        {
            try
            {
                // For demonstration, create sample attendance records
                var demoRecords = new List<AttendanceRecordViewModel>
        {
            new AttendanceRecordViewModel
            {
                Id = 1,
                Timestamp = DateTime.Parse("2025-03-19 04:30:00").ToUniversalTime(),
                LocationName = "Cognizant F3",
                IsPresent = true,
                Latitude = 13.049271m,
                Longitude = 77.6229m
            },
            new AttendanceRecordViewModel
            {
                Id = 2,
                Timestamp = DateTime.Parse("2025-03-18 00:15:00").ToUniversalTime(),
                LocationName = "Cognizant F3",
                IsPresent = true,
                Latitude = 13.049280m,
                Longitude = 77.6230m
            },
            new AttendanceRecordViewModel
            {
                Id = 3,
                Timestamp = DateTime.Parse("2025-03-18 04:45:00").ToUniversalTime(),
                LocationName = "Outside Office",
                IsPresent = false,
                Latitude = 13.052000m,
                Longitude = 77.6280m
            },
            new AttendanceRecordViewModel
            {
                Id = 4,
                Timestamp = DateTime.Parse("2025-03-10 00:30:00").ToUniversalTime(),
                LocationName = "Cognizant F3",
                IsPresent = true,
                Latitude = 13.049275m,
                Longitude = 77.6228m
            },
            new AttendanceRecordViewModel
            {
                Id = 5,
                Timestamp = DateTime.Parse("2025-03-09 04:55:00").ToUniversalTime(),
                LocationName = "Cognizant F3",
                IsPresent = true,
                Latitude = 13.049270m,
                Longitude = 77.6231m
            }
        };

                // Order by most recent first
                var orderedRecords = demoRecords.OrderByDescending(r => r.Timestamp);

                return View(orderedRecords);
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