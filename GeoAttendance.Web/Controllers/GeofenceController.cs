using Microsoft.AspNetCore.Mvc;
using GeoAttendance.Web.Models;
using GeoAttendance.Web.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GeoAttendance.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class GeofenceController : Controller
    {
        private readonly IGeofenceService _geofenceService;
        private readonly ILogger<GeofenceController> _logger;

        public GeofenceController(IGeofenceService geofenceService, ILogger<GeofenceController> logger)
        {
            _geofenceService = geofenceService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var geofences = await _geofenceService.GetAllGeofencesAsync();
                return View(geofences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching geofences");
                TempData["Error"] = "Could not load geofences. Please try again later.";
                return View(new List<GeofenceViewModel>());
            }
        }

        public IActionResult Create()
        {
            return View(new GeofenceViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GeofenceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _geofenceService.CreateGeofenceAsync(model);
                    TempData["Success"] = "Geofence created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating geofence");
                    ModelState.AddModelError("", "Could not create geofence. Please try again.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var geofence = await _geofenceService.GetGeofenceAsync(id);
                if (geofence == null)
                {
                    return NotFound();
                }
                return View(geofence);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching geofence {Id}", id);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GeofenceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _geofenceService.UpdateGeofenceAsync(model);
                    TempData["Success"] = "Geofence updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating geofence");
                    ModelState.AddModelError("", "Could not update geofence. Please try again.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var geofence = await _geofenceService.GetGeofenceAsync(id);
                if (geofence == null)
                {
                    return NotFound();
                }
                return View(geofence);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching geofence {Id} for deletion", id);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _geofenceService.DeleteGeofenceAsync(id);
                TempData["Success"] = "Geofence deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting geofence {Id}", id);
                TempData["Error"] = "Could not delete geofence. Please try again.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Map()
        {
            var geofences = await _geofenceService.GetAllGeofencesAsync();
            return View(geofences);
        }
    }
}