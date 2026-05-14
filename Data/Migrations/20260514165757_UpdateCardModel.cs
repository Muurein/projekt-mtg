using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projekt_mtg.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCardModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "SetId",
                table: "Cards");

            migrationBuilder.RenameColumn(
                name: "imageUrl",
                table: "Cards",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Cards",
                newName: "TypeLine");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Cards",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ManaValue",
                table: "Cards",
                newName: "OracleText");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Cards",
                newName: "ManaCost");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Cards",
                newName: "imageUrl");

            migrationBuilder.RenameColumn(
                name: "TypeLine",
                table: "Cards",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "OracleText",
                table: "Cards",
                newName: "ManaValue");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Cards",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ManaCost",
                table: "Cards",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Cards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetId",
                table: "Cards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
