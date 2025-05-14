using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class dangers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ActualTripLocations",
                newName: "Remain");

            migrationBuilder.AddColumn<long>(
                name: "JobSiteId",
                table: "PlannedTripLocations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "JobSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "ActualTripLocations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "PlannedTripLocationId",
                table: "ActualTripLocations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Received",
                table: "ActualTripLocations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "DangerCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangerCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Step = table.Column<int>(type: "int", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dangers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DangerCategoryId = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dangers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dangers_DangerCategories_DangerCategoryId",
                        column: x => x.DangerCategoryId,
                        principalTable: "DangerCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasureControls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DangerId = table.Column<int>(type: "int", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasureControls_Dangers_DangerId",
                        column: x => x.DangerId,
                        principalTable: "Dangers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlannedTripLocations_JobSiteId",
                table: "PlannedTripLocations",
                column: "JobSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualTripLocations_PlannedTripLocationId",
                table: "ActualTripLocations",
                column: "PlannedTripLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Dangers_DangerCategoryId",
                table: "Dangers",
                column: "DangerCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasureControls_DangerId",
                table: "MeasureControls",
                column: "DangerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActualTripLocations_PlannedTripLocations_PlannedTripLocationId",
                table: "ActualTripLocations",
                column: "PlannedTripLocationId",
                principalTable: "PlannedTripLocations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedTripLocations_JobSites_JobSiteId",
                table: "PlannedTripLocations",
                column: "JobSiteId",
                principalTable: "JobSites",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActualTripLocations_PlannedTripLocations_PlannedTripLocationId",
                table: "ActualTripLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedTripLocations_JobSites_JobSiteId",
                table: "PlannedTripLocations");

            migrationBuilder.DropTable(
                name: "MeasureControls");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Dangers");

            migrationBuilder.DropTable(
                name: "DangerCategories");

            migrationBuilder.DropIndex(
                name: "IX_PlannedTripLocations_JobSiteId",
                table: "PlannedTripLocations");

            migrationBuilder.DropIndex(
                name: "IX_ActualTripLocations_PlannedTripLocationId",
                table: "ActualTripLocations");

            migrationBuilder.DropColumn(
                name: "JobSiteId",
                table: "PlannedTripLocations");

            migrationBuilder.DropColumn(
                name: "City",
                table: "JobSites");

            migrationBuilder.DropColumn(
                name: "PlannedTripLocationId",
                table: "ActualTripLocations");

            migrationBuilder.DropColumn(
                name: "Received",
                table: "ActualTripLocations");

            migrationBuilder.RenameColumn(
                name: "Remain",
                table: "ActualTripLocations",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "ActualTripLocations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
