using GeoAttendance.API.Data;
using GeoAttendance.API.Services;
using GeoAttendance.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GeoAttendance.Tests.GeofenceTests;

public sealed class GeofenceCRUDTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly GeofenceService _geofenceService;
    private readonly ILogger<GeofenceService> _logger;

    public GeofenceCRUDTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _logger = Mock.Of<ILogger<GeofenceService>>();
        _geofenceService = new GeofenceService(_context, _logger);
    }

    [Fact]
    public async Task CreateGeofence_ValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var geofence = new Geofence
        {
            Name = "Test Geofence",
            Description = "Test Description",
            CenterLatitude = 13.0827,
            CenterLongitude = 80.2707,
            Radius = 100,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _geofenceService.CreateGeofenceAsync(geofence);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(geofence.Name, result.Name);
        Assert.Equal(geofence.CenterLatitude, result.CenterLatitude);
        Assert.Equal(geofence.CenterLongitude, result.CenterLongitude);
        Assert.Equal(geofence.Radius, result.Radius);
    }

    [Fact]
    public async Task GetGeofence_ExistingId_ShouldReturnGeofence()
    {
        // Arrange
        var geofence = new Geofence
        {
            Name = "Test Geofence",
            Description = "Test Description",
            CenterLatitude = 13.0827,
            CenterLongitude = 80.2707,
            Radius = 100,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Geofences.AddAsync(geofence);
        await _context.SaveChangesAsync();

        // Act
        var result = await _geofenceService.GetGeofenceAsync(geofence.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(geofence.Name, result.Name);
        Assert.Equal(geofence.CenterLatitude, result.CenterLatitude);
        Assert.Equal(geofence.CenterLongitude, result.CenterLongitude);
    }

    [Fact]
    public async Task UpdateGeofence_ValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var geofence = new Geofence
        {
            Name = "Test Geofence",
            Description = "Test Description",
            CenterLatitude = 13.0827,
            CenterLongitude = 80.2707,
            Radius = 100,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Geofences.AddAsync(geofence);
        await _context.SaveChangesAsync();

        // Update values
        geofence.Name = "Updated Geofence";
        geofence.Radius = 200;
        geofence.UpdatedAt = DateTime.UtcNow;

        // Act
        var result = await _geofenceService.UpdateGeofenceAsync(geofence);

        // Assert
        Assert.True(result);

        var updatedGeofence = await _context.Geofences.FindAsync(geofence.Id);
        Assert.NotNull(updatedGeofence);
        Assert.Equal("Updated Geofence", updatedGeofence.Name);
        Assert.Equal(200, updatedGeofence.Radius);
        Assert.NotNull(updatedGeofence.UpdatedAt);
    }

    [Fact]
    public async Task DeleteGeofence_ExistingId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var geofence = new Geofence
        {
            Name = "Test Geofence",
            Description = "Test Description",
            CenterLatitude = 13.0827,
            CenterLongitude = 80.2707,
            Radius = 100,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Geofences.AddAsync(geofence);
        await _context.SaveChangesAsync();

        // Act
        var result = await _geofenceService.DeleteGeofenceAsync(geofence.Id);

        // Assert
        Assert.True(result);
        var deletedGeofence = await _context.Geofences.FindAsync(geofence.Id);
        Assert.Null(deletedGeofence);
    }

    [Fact]
    public async Task GetAllGeofences_ShouldReturnAllActiveGeofences()
    {
        // Arrange
        var geofences = new List<Geofence>
        {
            new()
            {
                Name = "Geofence 1",
                Description = "Test Description 1",
                CenterLatitude = 13.0827,
                CenterLongitude = 80.2707,
                Radius = 100,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Geofence 2",
                Description = "Test Description 2",
                CenterLatitude = 13.0828,
                CenterLongitude = 80.2708,
                Radius = 150,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Geofences.AddRangeAsync(geofences);
        await _context.SaveChangesAsync();

        // Act
        var result = await _geofenceService.GetAllGeofencesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, g => g.Name == "Geofence 1");
        Assert.Contains(result, g => g.Name == "Geofence 2");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
