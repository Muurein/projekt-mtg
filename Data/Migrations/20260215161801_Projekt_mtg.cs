using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projekt_mtg.Data.Migrations
{
    /// <inheritdoc />
    public partial class Projekt_mtg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Quantity",
                table: "Cards",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Cards",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cards");
        }
    }
}
