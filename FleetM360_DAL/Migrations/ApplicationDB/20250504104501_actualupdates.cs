using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class actualupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripDangers_MeasureControls_MeasureControlId",
                table: "TripDangers");

            migrationBuilder.DropIndex(
                name: "IX_TripDangers_MeasureControlId",
                table: "TripDangers");

            migrationBuilder.DropColumn(
                name: "MeasureControlId",
                table: "TripDangers");

            migrationBuilder.AlterColumn<double>(
                name: "Long",
                table: "TripDangers",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                table: "TripDangers",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "MeasureControl",
                table: "TripDangers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "ActualTripLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Minutes",
                table: "ActualTripLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Seconds",
                table: "ActualTripLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TripLogId",
                table: "ActualTripLocations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "TripTake5s",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userNumber = table.Column<long>(type: "bigint", nullable: false),
                    truckId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tripId = table.Column<long>(type: "bigint", nullable: false),
                    TripLogId = table.Column<long>(type: "bigint", nullable: false),
                    ActualTripId = table.Column<long>(type: "bigint", nullable: false),
                    Step = table.Column<int>(type: "int", nullable: false),
                    lat = table.Column<double>(type: "float", nullable: false),
                    lng = table.Column<double>(type: "float", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripTake5s", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripTake5s");

            migrationBuilder.DropColumn(
                name: "MeasureControl",
                table: "TripDangers");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "ActualTripLocations");

            migrationBuilder.DropColumn(
                name: "Minutes",
                table: "ActualTripLocations");

            migrationBuilder.DropColumn(
                name: "Seconds",
                table: "ActualTripLocations");

            migrationBuilder.DropColumn(
                name: "TripLogId",
                table: "ActualTripLocations");

            migrationBuilder.AlterColumn<double>(
                name: "Long",
                table: "TripDangers",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                table: "TripDangers",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MeasureControlId",
                table: "TripDangers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TripDangers_MeasureControlId",
                table: "TripDangers",
                column: "MeasureControlId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripDangers_MeasureControls_MeasureControlId",
                table: "TripDangers",
                column: "MeasureControlId",
                principalTable: "MeasureControls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
