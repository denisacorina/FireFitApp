using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireFitBlazor.Migrations
{
    /// <inheritdoc />
    public partial class GoalChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DietaryPreference",
                table: "Goals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DietaryPreference",
                table: "Goals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
