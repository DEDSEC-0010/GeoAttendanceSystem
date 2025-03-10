using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoAttendance.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Radius",
                table: "Geofences",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "CenterLongitude",
                table: "Geofences",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 9,
                oldScale: 6);

            migrationBuilder.AlterColumn<double>(
                name: "CenterLatitude",
                table: "Geofences",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 9,
                oldScale: 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Radius",
                table: "Geofences",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "CenterLongitude",
                table: "Geofences",
                type: "real",
                precision: 9,
                scale: 6,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 6);

            migrationBuilder.AlterColumn<float>(
                name: "CenterLatitude",
                table: "Geofences",
                type: "real",
                precision: 9,
                scale: 6,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 6);
        }
    }
}
