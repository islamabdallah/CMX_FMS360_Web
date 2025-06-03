using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class sapTripTabbleUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivedDate",
                table: "SapTrips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customerNumber",
                table: "SapTrips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "departureDate",
                table: "SapTrips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "materialNumber",
                table: "SapTrips",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivedDate",
                table: "SapTrips");

            migrationBuilder.DropColumn(
                name: "customerNumber",
                table: "SapTrips");

            migrationBuilder.DropColumn(
                name: "departureDate",
                table: "SapTrips");

            migrationBuilder.DropColumn(
                name: "materialNumber",
                table: "SapTrips");
        }
    }
}
