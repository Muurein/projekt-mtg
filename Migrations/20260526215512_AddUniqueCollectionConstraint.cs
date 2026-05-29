using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projekt_mtg.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueCollectionConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Collections_CardId",
                table: "Collections");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_CardId_UserId",
                table: "Collections",
                columns: new[] { "CardId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Collections_CardId_UserId",
                table: "Collections");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_CardId",
                table: "Collections",
                column: "CardId");
        }
    }
}
