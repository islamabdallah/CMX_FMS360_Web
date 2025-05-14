using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class TripDangersApdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripDangers_JobSites_JobSiteId",
                table: "TripDangers");

            migrationBuilder.DropForeignKey(
                name: "FK_TripQuestions_JobSites_JobSiteId",
                table: "TripQuestions");

            migrationBuilder.DropIndex(
                name: "IX_TripQuestions_JobSiteId",
                table: "TripQuestions");

            migrationBuilder.DropIndex(
                name: "IX_TripDangers_JobSiteId",
                table: "TripDangers");

            migrationBuilder.DropColumn(
                name: "JobSiteId",
                table: "TripQuestions");

            migrationBuilder.DropColumn(
                name: "JobSiteId",
                table: "TripDangers");

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "TripQuestions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Long",
                table: "TripQuestions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "TripDangers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Long",
                table: "TripDangers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "TripQuestions");

            migrationBuilder.DropColumn(
                name: "Long",
                table: "TripQuestions");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "TripDangers");

            migrationBuilder.DropColumn(
                name: "Long",
                table: "TripDangers");

            migrationBuilder.AddColumn<long>(
                name: "JobSiteId",
                table: "TripQuestions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "JobSiteId",
                table: "TripDangers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TripQuestions_JobSiteId",
                table: "TripQuestions",
                column: "JobSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TripDangers_JobSiteId",
                table: "TripDangers",
                column: "JobSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripDangers_JobSites_JobSiteId",
                table: "TripDangers",
                column: "JobSiteId",
                principalTable: "JobSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripQuestions_JobSites_JobSiteId",
                table: "TripQuestions",
                column: "JobSiteId",
                principalTable: "JobSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
