using GeoAttendance.API.Data;
using GeoAttendance.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GeoAttendance.API.Services;

public class GeofenceService
{
    private readonly AppDbContext _context;
    private readonly ILogger<GeofenceService> _logger;
    private const decimal EarthRadiusMeters = 6371000m; // Earth's radius in meters

    public GeofenceService(AppDbContext context, ILogger<GeofenceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Geofence> CreateGeofenceAsync(Geofence geofence)
    {
        geofence.CreatedAt = DateTime.UtcNow;
        _context.Geofences.Add(geofence);
        await _context.SaveChangesAsync();
        return geofence;
    }

    public async Task<Geofence> GetGeofenceAsync(int id)
    {
        return await _context.Geofences.FindAsync(id);
    }

    public async Task<List<Geofence>> GetAllGeofencesAsync()
    {
        return await _context.Geofences
            .Where(g => g.IsActive)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> UpdateGeofenceAsync(Geofence geofence)
    {
        geofence.UpdatedAt = DateTime.UtcNow;
        _context.Geofences.Update(geofence);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteGeofenceAsync(int id)
    {
        var geofence = await _context.Geofences.FindAsync(id);
        if (geofence == null) return false;

        _context.Geofences.Remove(geofence);
        return await _context.SaveChangesAsync() > 0;
    }

    public bool IsLocationWithinGeofence(decimal latitude, decimal longitude, Geofence geofence)
    {
        try
        {
            // Input validation
            if (!IsValidLatitude(latitude) || !IsValidLongitude(longitude))
            {
                _logger.LogWarning("Invalid coordinates: Lat {Latitude}, Lon {Longitude}",
                    latitude, longitude);
                return false;
            }

            if (!IsValidLatitude((decimal)geofence.CenterLatitude) ||
                !IsValidLongitude((decimal)geofence.CenterLongitude))
            {
                _logger.LogWarning("Invalid geofence coordinates: Lat {Latitude}, Lon {Longitude}",
                    geofence.CenterLatitude, geofence.CenterLongitude);
                return false;
            }

            // Calculate using decimal arithmetic for better precision
            var latRad1 = ToRadians(latitude);
            var lonRad1 = ToRadians(longitude);
            var latRad2 = ToRadians((decimal)geofence.CenterLatitude);
            var lonRad2 = ToRadians((decimal)geofence.CenterLongitude);

            var deltaLat = latRad2 - latRad1;
            var deltaLon = lonRad2 - lonRad1;

            // Haversine formula with decimal arithmetic
            var a = DecimalSin(deltaLat / 2m) * DecimalSin(deltaLat / 2m) +
                   DecimalCos(latRad1) * DecimalCos(latRad2) *
                   DecimalSin(deltaLon / 2m) * DecimalSin(deltaLon / 2m);

            var c = 2m * DecimalAtan2(DecimalSqrt(a), DecimalSqrt(1m - a));
            var distance = EarthRadiusMeters * c;

            var isWithin = distance <= (decimal)geofence.Radius;

            _logger.LogInformation(
                "Location check - Lat: {Latitude}, Lon: {Longitude}, Distance: {Distance}m, " +
                "Radius: {Radius}m, IsWithin: {IsWithin}",
                latitude, longitude, distance, geofence.Radius, isWithin);

            return isWithin;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating distance for geofence {GeofenceId}",
                geofence.Id);
            return false;
        }
    }

    public async Task<bool> IsLocationWithinAnyGeofenceAsync(decimal latitude, decimal longitude)
    {
        try
        {
            var activeGeofences = await GetAllGeofencesAsync();

            if (!activeGeofences.Any())
            {
                _logger.LogWarning("No active geofences found");
                return false;
            }

            foreach (var geofence in activeGeofences)
            {
                if (IsLocationWithinGeofence(latitude, longitude, geofence))
                {
                    _logger.LogInformation(
                        "Location ({Latitude}, {Longitude}) is within geofence {GeofenceName}",
                        latitude, longitude, geofence.Name);
                    return true;
                }
            }

            _logger.LogInformation(
                "Location ({Latitude}, {Longitude}) is not within any geofence",
                latitude, longitude);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking location within geofences");
            throw;
        }
    }

    private static bool IsValidLatitude(decimal latitude)
    {
        return latitude >= -90m && latitude <= 90m;
    }

    private static bool IsValidLongitude(decimal longitude)
    {
        return longitude >= -180m && longitude <= 180m;
    }

    private static decimal ToRadians(decimal degrees)
    {
        return degrees * (decimal)Math.PI / 180m;
    }

    // Decimal versions of Math functions for better precision
    private static decimal DecimalSin(decimal x)
    {
        return (decimal)Math.Sin((double)x);
    }

    private static decimal DecimalCos(decimal x)
    {
        return (decimal)Math.Cos((double)x);
    }

    private static decimal DecimalSqrt(decimal x)
    {
        return (decimal)Math.Sqrt((double)x);
    }

    private static decimal DecimalAtan2(decimal y, decimal x)
    {
        return (decimal)Math.Atan2((double)y, (double)x);
    }
}