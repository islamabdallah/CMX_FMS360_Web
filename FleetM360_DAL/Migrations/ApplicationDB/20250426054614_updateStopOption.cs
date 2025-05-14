using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class updateStopOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "label",
                table: "StopOptions",
                newName: "Label_AR");

            migrationBuilder.AddColumn<string>(
                name: "Label_EN",
                table: "StopOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Label_EN",
                table: "StopOptions");

            migrationBuilder.RenameColumn(
                name: "Label_AR",
                table: "StopOptions",
                newName: "label");
        }
    }
}
