using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabTrainer.Api.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguagefromadmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CorrectCount",
                table: "QuizSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "QuizSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalItems",
                table: "QuizSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WrongCount",
                table: "QuizSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Attempt",
                table: "QuizItemResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SelectedAnswer",
                table: "QuizItemResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Languages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectCount",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "TotalItems",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "WrongCount",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "Attempt",
                table: "QuizItemResults");

            migrationBuilder.DropColumn(
                name: "SelectedAnswer",
                table: "QuizItemResults");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Languages");
        }
    }
}
