using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class fromPlant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FromPlant",
                table: "Trips",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromPlant",
                table: "Trips");
        }
    }
}
