using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabTrainer.Api.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddSummaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LanguageId",
                table: "QuizItemResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "QuizItemResults",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserLanguageStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalWords = table.Column<int>(type: "int", nullable: false),
                    DueWords = table.Column<int>(type: "int", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    Accuracy = table.Column<double>(type: "float", nullable: false),
                    LastQuizAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastQuizTotalQuestions = table.Column<int>(type: "int", nullable: true),
                    LastQuizCorrectAnswers = table.Column<int>(type: "int", nullable: true),
                    LastQuizAccuracy = table.Column<double>(type: "float", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLanguageStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLanguageStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLanguageStats_UserId_LanguageId",
                table: "UserLanguageStats",
                columns: new[] { "UserId", "LanguageId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLanguageStats");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "QuizItemResults");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuizItemResults");
        }
    }
}
