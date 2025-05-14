using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class navigationAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreCheckAnswers_Questions_QuestionId",
                table: "PreCheckAnswers");

            migrationBuilder.DropIndex(
                name: "IX_PreCheckAnswers_QuestionId",
                table: "PreCheckAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "PreCheckAnswers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "PreCheckAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreCheckAnswers_QuestionId",
                table: "PreCheckAnswers",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreCheckAnswers_Questions_QuestionId",
                table: "PreCheckAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }
    }
}
