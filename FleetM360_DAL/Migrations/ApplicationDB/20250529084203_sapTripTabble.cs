using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class sapTripTabble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AssignQty",
                table: "Trips",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MustStart",
                table: "Trips",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConverted",
                table: "TripLogs",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SapTrips",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripNumber = table.Column<long>(type: "bigint", nullable: false),
                    TruckNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<double>(type: "float", nullable: false),
                    jobsiteNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SapTrips", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SapTrips");

            migrationBuilder.DropColumn(
                name: "AssignQty",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "MustStart",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsConverted",
                table: "TripLogs");
        }
    }
}
