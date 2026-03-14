using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabTrainer.Api.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class editSummaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TranslatedWords",
                table: "UserLanguageStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UntranslatedWords",
                table: "UserLanguageStats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslatedWords",
                table: "UserLanguageStats");

            migrationBuilder.DropColumn(
                name: "UntranslatedWords",
                table: "UserLanguageStats");
        }
    }
}
