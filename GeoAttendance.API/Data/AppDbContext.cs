using Microsoft.EntityFrameworkCore;
using GeoAttendance.Common.Models;

namespace GeoAttendance.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<Geofence> Geofences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Geofence>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CenterLatitude)
                .IsRequired()
                .HasPrecision(9, 6);

            entity.Property(e => e.CenterLongitude)
                .IsRequired()
                .HasPrecision(9, 6);

            entity.Property(e => e.Radius)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .IsRequired(false);
        });

        // Add index on IsActive to optimize queries for active geofences
        modelBuilder.Entity<Geofence>()
            .HasIndex(g => g.IsActive);

        // Add spatial index on coordinates for better performance on location queries
        modelBuilder.Entity<Geofence>()
            .HasIndex(g => new { g.CenterLatitude, g.CenterLongitude });
    }
}