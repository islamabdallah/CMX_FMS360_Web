using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class TripDangers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TripDangers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<long>(type: "bigint", nullable: false),
                    ParentTrip = table.Column<long>(type: "bigint", nullable: false),
                    TripNumber = table.Column<long>(type: "bigint", nullable: false),
                    JobSiteId = table.Column<long>(type: "bigint", nullable: false),
                    MeasureControlId = table.Column<long>(type: "bigint", nullable: false),
                    DangerId = table.Column<int>(type: "int", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripDangers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripDangers_JobSites_JobSiteId",
                        column: x => x.JobSiteId,
                        principalTable: "JobSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripDangers_MeasureControls_MeasureControlId",
                        column: x => x.MeasureControlId,
                        principalTable: "MeasureControls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripDangers_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<long>(type: "bigint", nullable: false),
                    ParentTrip = table.Column<long>(type: "bigint", nullable: false),
                    TripNumber = table.Column<long>(type: "bigint", nullable: false),
                    JobSiteId = table.Column<long>(type: "bigint", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<bool>(type: "bit", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripQuestions_JobSites_JobSiteId",
                        column: x => x.JobSiteId,
                        principalTable: "JobSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripQuestions_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripDangers_JobSiteId",
                table: "TripDangers",
                column: "JobSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TripDangers_MeasureControlId",
                table: "TripDangers",
                column: "MeasureControlId");

            migrationBuilder.CreateIndex(
                name: "IX_TripDangers_TripId",
                table: "TripDangers",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_TripQuestions_JobSiteId",
                table: "TripQuestions",
                column: "JobSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TripQuestions_QuestionId",
                table: "TripQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TripQuestions_TripId",
                table: "TripQuestions",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripDangers");

            migrationBuilder.DropTable(
                name: "TripQuestions");
        }
    }
}
