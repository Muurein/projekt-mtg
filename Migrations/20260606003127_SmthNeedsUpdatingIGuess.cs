using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projekt_mtg.Migrations
{
    /// <inheritdoc />
    public partial class SmthNeedsUpdatingIGuess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollUsers");

            migrationBuilder.DropIndex(
                name: "IX_DeckCards_DeckId",
                table: "DeckCards");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_DeckId_CardId",
                table: "DeckCards",
                columns: new[] { "DeckId", "CardId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeckCards_DeckId_CardId",
                table: "DeckCards");

            migrationBuilder.CreateTable(
                name: "CollUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_DeckId",
                table: "DeckCards",
                column: "DeckId");
        }
    }
}
