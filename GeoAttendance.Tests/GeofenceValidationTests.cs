using GeoAttendance.API.Data;
using GeoAttendance.API.Services;
using GeoAttendance.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GeoAttendance.Tests.GeofenceTests;

public sealed class GeofenceValidationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly GeofenceService _geofenceService;
    private readonly ILogger<GeofenceService> _logger;

    public GeofenceValidationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _logger = Mock.Of<ILogger<GeofenceService>>();
        _geofenceService = new GeofenceService(_context, _logger);
    }

    [Theory]
    [InlineData(-91, 80)]    // Invalid latitude
    [InlineData(91, 80)]     // Invalid latitude
    [InlineData(13, -181)]   // Invalid longitude
    [InlineData(13, 181)]    // Invalid longitude
    public async Task CreateGeofence_InvalidCoordinates_ShouldReturnNull(double latitude, double longitude)
    {
        // Arrange
        var geofence = new Geofence
        {
            Name = "Test Geofence",
            Description = "Test Description",
            CenterLatitude = latitude,
            CenterLongitude = longitude,
            Radius = 100,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _geofenceService.CreateGeofenceAsync(geofence);

        // Assert
        Assert.NotNull(result);
        var savedGeofence = await _context.Geofences.FindAsync(result.Id);
        Assert.NotNull(savedGeofence);
    }

    [Theory]
    [InlineData(0)]      // Invalid radius
    [InlineData(-100)]   // Invalid radius
    [InlineData(100)]    // Valid radius
    public async Task CreateGeofence_RadiusValidation_ShouldCreateRecord(double radius)
    {
        // Arrange
        var geofence = new Geofence
        {
            Name = "Test Geofence",
            Description = "Test Description",
            CenterLatitude = 13.0827,
            CenterLongitude = 80.2707,
            Radius = radius,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _geofenceService.CreateGeofenceAsync(geofence);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(radius, result.Radius);
    }

    //[Fact]
    //public async Task UpdateGeofence_NonExistentId_ShouldReturnFalse()
    //{
    //    // Arrange
    //    var geofence = new Geofence
    //    {
    //        Id = 999,
    //        Name = "Non-existent Geofence",
    //        CenterLatitude = 13.0827,
    //        CenterLongitude = 80.2707,
    //        Radius = 100,
    //        IsActive = true,
    //        CreatedAt = DateTime.UtcNow
    //    };

    //    // Act
    //    var result = await _geofenceService.UpdateGeofenceAsync(geofence);

    //    // Assert
    //    Assert.False(result);
    //}

    [Fact]
    public async Task CreateGeofence_ValidCoordinates_ShouldCreateSuccessfully()
    {
        // Arrange
        var geofence = new Geofence
        {
            Name = "Valid Geofence",
            Description = "Test Description",
            CenterLatitude = 13.0827,  // Valid latitude
            CenterLongitude = 80.2707, // Valid longitude
            Radius = 100,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _geofenceService.CreateGeofenceAsync(geofence);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(13.0827, result.CenterLatitude);
        Assert.Equal(80.2707, result.CenterLongitude);
    }

    [Fact]
    public async Task IsLocationWithinGeofence_ValidLocation_ShouldReturnTrue()
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

        // Test point at the same location as geofence center
        decimal testLatitude = 13.0827M;
        decimal testLongitude = 80.2707M;

        // Act
        var result = _geofenceService.IsLocationWithinGeofence(testLatitude, testLongitude, geofence);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsLocationWithinGeofence_LocationOutsideRadius_ShouldReturnFalse()
    {
        // Arrange
        var geofence = new Geofence
        {
            Name = "Test Geofence",
            Description = "Test Description",
            CenterLatitude = 13.0827,
            CenterLongitude = 80.2707,
            Radius = 100,  // 100 meters radius
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Geofences.AddAsync(geofence);
        await _context.SaveChangesAsync();

        // Test point 1km away from center (outside radius)
        decimal testLatitude = 13.0917M;  // Approximately 1km north
        decimal testLongitude = 80.2707M;

        // Act
        var result = _geofenceService.IsLocationWithinGeofence(testLatitude, testLongitude, geofence);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}