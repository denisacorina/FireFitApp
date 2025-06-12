using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireFitBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddedFoodDataset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContainsAnimalProducts",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "ContainsGluten",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "ContainsLactose",
                table: "Ingredient");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Ingredient",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Ingredient");

            migrationBuilder.AddColumn<bool>(
                name: "ContainsAnimalProducts",
                table: "Ingredient",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ContainsGluten",
                table: "Ingredient",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ContainsLactose",
                table: "Ingredient",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
