using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeoAttendance.API.Services;
using GeoAttendance.API.DTOs;
using GeoAttendance.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoAttendance.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GeofenceController : ControllerBase
{
    private readonly GeofenceService _geofenceService;
    private readonly ILogger<GeofenceController> _logger;

    public GeofenceController(GeofenceService geofenceService, ILogger<GeofenceController> logger)
    {
        _geofenceService = geofenceService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GeofenceResponseDTO>> CreateGeofence([FromBody] GeofenceDTO dto)
    {
        try
        {
            _logger.LogInformation("Creating new geofence with name: {Name}", dto.Name);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for geofence creation");
                return BadRequest(ModelState);
            }

            var geofence = new Geofence
            {
                Name = dto.Name,
                CenterLatitude = dto.CenterLatitude,
                CenterLongitude = dto.CenterLongitude,
                Radius = dto.Radius,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            var createdGeofence = await _geofenceService.CreateGeofenceAsync(geofence);
            
            _logger.LogInformation("Successfully created geofence with ID: {Id}", createdGeofence.Id);

            return CreatedAtAction(
                nameof(GetGeofence), 
                new { id = createdGeofence.Id }, 
                MapToResponseDTO(createdGeofence)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating geofence with name: {Name}", dto?.Name ?? "unknown");
            return StatusCode(500, new { error = ex.Message, detail = ex.InnerException?.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GeofenceResponseDTO>> GetGeofence(int id)
    {
        try
        {
            _logger.LogInformation("Fetching geofence with ID: {Id}", id);
            
            var geofence = await _geofenceService.GetGeofenceAsync(id);
            if (geofence == null)
            {
                _logger.LogWarning("Geofence not found with ID: {Id}", id);
                return NotFound();
            }

            return Ok(MapToResponseDTO(geofence));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching geofence with ID: {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GeofenceResponseDTO>>> GetAllGeofences()
    {
        try
        {
            _logger.LogInformation("Fetching all active geofences");
            
            var geofences = await _geofenceService.GetAllGeofencesAsync();
            return Ok(geofences.Select(MapToResponseDTO));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all geofences");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateGeofence(int id, [FromBody] UpdateGeofenceDTO dto)
    {
        try
        {
            _logger.LogInformation("Updating geofence with ID: {Id}", id);

            if (id != dto.Id)
            {
                _logger.LogWarning("ID mismatch in update request. Path ID: {PathId}, DTO ID: {DtoId}", id, dto.Id);
                return BadRequest("ID mismatch");
            }

            var existingGeofence = await _geofenceService.GetGeofenceAsync(id);
            if (existingGeofence == null)
            {
                _logger.LogWarning("Geofence not found for update with ID: {Id}", id);
                return NotFound();
            }

            existingGeofence.Name = dto.Name;
            existingGeofence.CenterLatitude = dto.CenterLatitude;
            existingGeofence.CenterLongitude = dto.CenterLongitude;
            existingGeofence.Radius = dto.Radius;
            existingGeofence.Description = dto.Description;
            existingGeofence.IsActive = dto.IsActive;
            existingGeofence.UpdatedAt = DateTime.UtcNow;

            var success = await _geofenceService.UpdateGeofenceAsync(existingGeofence);
            if (!success)
            {
                _logger.LogError("Failed to update geofence with ID: {Id}", id);
                return StatusCode(500, "Failed to update geofence");
            }

            _logger.LogInformation("Successfully updated geofence with ID: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating geofence with ID: {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("check-location")]
    public async Task<ActionResult<bool>> CheckLocation([FromBody] GeofenceLocationCheckDTO location)
    {
        try
        {
            _logger.LogInformation("Checking location: Lat={Latitude}, Long={Longitude}", 
                location.Latitude, location.Longitude);

            var isWithinGeofence = await _geofenceService.IsLocationWithinAnyGeofenceAsync(
                location.Latitude,
                location.Longitude
            );
            
            _logger.LogInformation("Location check result: {Result}", 
                isWithinGeofence ? "Within geofence" : "Outside geofence");

            return Ok(isWithinGeofence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking location");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private static GeofenceResponseDTO MapToResponseDTO(Geofence geofence)
    {
        return new GeofenceResponseDTO
        {
            Id = geofence.Id,
            Name = geofence.Name,
            CenterLatitude = geofence.CenterLatitude,
            CenterLongitude = geofence.CenterLongitude,
            Radius = geofence.Radius,
            Description = geofence.Description,
            IsActive = geofence.IsActive,
            CreatedAt = geofence.CreatedAt,
            UpdatedAt = geofence.UpdatedAt
        };
    }
}