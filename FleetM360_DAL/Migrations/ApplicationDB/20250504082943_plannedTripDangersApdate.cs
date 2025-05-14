using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class plannedTripDangersApdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripDangers_Trips_TripId",
                table: "TripDangers");

            migrationBuilder.DropForeignKey(
                name: "FK_TripQuestions_Trips_TripId",
                table: "TripQuestions");

            migrationBuilder.RenameColumn(
                name: "TripId",
                table: "TripQuestions",
                newName: "PlannedTripLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_TripQuestions_TripId",
                table: "TripQuestions",
                newName: "IX_TripQuestions_PlannedTripLocationId");

            migrationBuilder.RenameColumn(
                name: "TripId",
                table: "TripDangers",
                newName: "PlannedTripLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_TripDangers_TripId",
                table: "TripDangers",
                newName: "IX_TripDangers_PlannedTripLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripDangers_PlannedTripLocations_PlannedTripLocationId",
                table: "TripDangers",
                column: "PlannedTripLocationId",
                principalTable: "PlannedTripLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripQuestions_PlannedTripLocations_PlannedTripLocationId",
                table: "TripQuestions",
                column: "PlannedTripLocationId",
                principalTable: "PlannedTripLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripDangers_PlannedTripLocations_PlannedTripLocationId",
                table: "TripDangers");

            migrationBuilder.DropForeignKey(
                name: "FK_TripQuestions_PlannedTripLocations_PlannedTripLocationId",
                table: "TripQuestions");

            migrationBuilder.RenameColumn(
                name: "PlannedTripLocationId",
                table: "TripQuestions",
                newName: "TripId");

            migrationBuilder.RenameIndex(
                name: "IX_TripQuestions_PlannedTripLocationId",
                table: "TripQuestions",
                newName: "IX_TripQuestions_TripId");

            migrationBuilder.RenameColumn(
                name: "PlannedTripLocationId",
                table: "TripDangers",
                newName: "TripId");

            migrationBuilder.RenameIndex(
                name: "IX_TripDangers_PlannedTripLocationId",
                table: "TripDangers",
                newName: "IX_TripDangers_TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripDangers_Trips_TripId",
                table: "TripDangers",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripQuestions_Trips_TripId",
                table: "TripQuestions",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
