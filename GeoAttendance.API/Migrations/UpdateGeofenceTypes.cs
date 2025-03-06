using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoAttendance.API.Migrations;

public partial class UpdateGeofenceTypes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Drop existing table
        migrationBuilder.Sql("IF OBJECT_ID('Geofences', 'U') IS NOT NULL DROP TABLE Geofences;");

        // Create new table with correct types
        migrationBuilder.CreateTable(
            name: "Geofences",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                CenterLatitude = table.Column<float>(type: "real", nullable: false),
                CenterLongitude = table.Column<float>(type: "real", nullable: false),
                Radius = table.Column<float>(type: "real", nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            });

        migrationBuilder.AddPrimaryKey(
            name: "PK_Geofences",
            table: "Geofences",
            column: "Id");

        // Simplified indexes without precision/scale
        migrationBuilder.CreateIndex(
            name: "IX_Geofences_IsActive",
            table: "Geofences",
            column: "IsActive");

        migrationBuilder.CreateIndex(
            name: "IX_Geofences_CenterLatitude_CenterLongitude",
            table: "Geofences",
            columns: new[] { "CenterLatitude", "CenterLongitude" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Geofences");
    }
}