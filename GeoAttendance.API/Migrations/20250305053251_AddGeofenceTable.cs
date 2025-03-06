using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoAttendance.API.Migrations;

public partial class AddGeofenceTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Drop existing indexes if they exist
        migrationBuilder.Sql(@"
            IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Geofences_IsActive' AND object_id = OBJECT_ID('Geofences'))
                DROP INDEX IX_Geofences_IsActive ON Geofences;
            
            IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Geofences_CenterLatitude_CenterLongitude' AND object_id = OBJECT_ID('Geofences'))
                DROP INDEX IX_Geofences_CenterLatitude_CenterLongitude ON Geofences;
        ");

        // Drop existing table if it exists
        migrationBuilder.Sql("IF OBJECT_ID('Geofences', 'U') IS NOT NULL DROP TABLE Geofences;");

        // Create new table
        migrationBuilder.CreateTable(
            name: "Geofences",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                CenterLatitude = table.Column<double>(type: "float", nullable: false),
                CenterLongitude = table.Column<double>(type: "float", nullable: false),
                Radius = table.Column<double>(type: "float", nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            });

        // Add primary key
        migrationBuilder.AddPrimaryKey(
            name: "PK_Geofences",
            table: "Geofences",
            column: "Id");

        // Add indexes
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
        migrationBuilder.DropTable(name: "Geofences");
    }
}