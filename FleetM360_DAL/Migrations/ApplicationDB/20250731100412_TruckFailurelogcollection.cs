using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class TruckFailurelogcollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TruckFailures_TruckFailures_TruckFailureId",
                table: "TruckFailures");

            migrationBuilder.DropIndex(
                name: "IX_TruckFailures_TruckFailureId",
                table: "TruckFailures");

            migrationBuilder.DropColumn(
                name: "TruckFailureId",
                table: "TruckFailures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TruckFailureId",
                table: "TruckFailures",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TruckFailures_TruckFailureId",
                table: "TruckFailures",
                column: "TruckFailureId");

            migrationBuilder.AddForeignKey(
                name: "FK_TruckFailures_TruckFailures_TruckFailureId",
                table: "TruckFailures",
                column: "TruckFailureId",
                principalTable: "TruckFailures",
                principalColumn: "Id");
        }
    }
}
