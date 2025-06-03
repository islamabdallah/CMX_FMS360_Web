using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class locationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "customerName",
                table: "PlannedTripLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customerPhoneNumber",
                table: "PlannedTripLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "locationStatus",
                table: "PlannedTripLocations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipientName",
                table: "PlannedTripLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipientPhoneNumber",
                table: "PlannedTripLocations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customerName",
                table: "PlannedTripLocations");

            migrationBuilder.DropColumn(
                name: "customerPhoneNumber",
                table: "PlannedTripLocations");

            migrationBuilder.DropColumn(
                name: "locationStatus",
                table: "PlannedTripLocations");

            migrationBuilder.DropColumn(
                name: "recipientName",
                table: "PlannedTripLocations");

            migrationBuilder.DropColumn(
                name: "recipientPhoneNumber",
                table: "PlannedTripLocations");
        }
    }
}
