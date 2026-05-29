using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projekt_mtg.Migrations
{
    /// <inheritdoc />
    public partial class DeckCardsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCommander",
                table: "DeckCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCommander",
                table: "DeckCards");
        }
    }
}
