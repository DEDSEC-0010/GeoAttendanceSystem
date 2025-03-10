﻿using GeoAttendance.API.Data;
using GeoAttendance.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoAttendance.API.Services;

public class GeofenceService
{
    private readonly AppDbContext _context;

    public GeofenceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Geofence> CreateGeofenceAsync(Geofence geofence)
    {
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
        return await _context.Geofences.Where(g => g.IsActive).ToListAsync();
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
        // Convert decimal to double for calculations
        double lat1 = (double)latitude;
        double lon1 = (double)longitude;
        double lat2 = (double)geofence.CenterLatitude;
        double lon2 = (double)geofence.CenterLongitude;
        double radius = (double)geofence.Radius;

        const double earthRadius = 6371000; // Earth's radius in meters

        var latRad1 = ToRadians(lat1);
        var latRad2 = ToRadians(lat2);
        var deltaLat = ToRadians(lat2 - lat1);
        var deltaLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(latRad1) * Math.Cos(latRad2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = earthRadius * c;

        return distance <= radius;
    }

    public async Task<bool> IsLocationWithinAnyGeofenceAsync(decimal latitude, decimal longitude)
    {
        var activeGeofences = await GetAllGeofencesAsync();
        return activeGeofences.Any(geofence => IsLocationWithinGeofence(latitude, longitude, geofence));
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}