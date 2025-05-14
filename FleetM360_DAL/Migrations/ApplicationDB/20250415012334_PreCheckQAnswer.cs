using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetM360_DAL.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class PreCheckQAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreCheckQuestions",
                columns: table => new
                {
                    PreCheckQuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreCheckQuestions", x => x.PreCheckQuestionId);
                });

            migrationBuilder.CreateTable(
                name: "PreCheckAnswers",
                columns: table => new
                {
                    PreCheckAnswerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerNameEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerNameAR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerValue = table.Column<bool>(type: "bit", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreCheckQuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreCheckAnswers", x => x.PreCheckAnswerId);
                    table.ForeignKey(
                        name: "FK_PreCheckAnswers_PreCheckQuestions_PreCheckQuestionId",
                        column: x => x.PreCheckQuestionId,
                        principalTable: "PreCheckQuestions",
                        principalColumn: "PreCheckQuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreCheckAnswers_PreCheckQuestionId",
                table: "PreCheckAnswers",
                column: "PreCheckQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreCheckAnswers");

            migrationBuilder.DropTable(
                name: "PreCheckQuestions");
        }
    }
}
