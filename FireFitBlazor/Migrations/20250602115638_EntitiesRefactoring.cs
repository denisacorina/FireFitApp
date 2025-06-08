using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireFitBlazor.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_Users_UserId",
                table: "UserPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProgress_Users_UserId",
                table: "UserProgress");

            migrationBuilder.DropIndex(
                name: "IX_UserProgress_UserId",
                table: "UserProgress");

            migrationBuilder.DropColumn(
                name: "DailyCalorieGoal",
                table: "UserPreferences");

            migrationBuilder.AddColumn<decimal>(
                name: "StartingWeight",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "User_TargetWeight",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserProgress",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartingWeight",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "User_TargetWeight",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserProgress",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DailyCalorieGoal",
                table: "UserPreferences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_UserId",
                table: "UserProgress",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_Users_UserId",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgress_Users_UserId",
                table: "UserProgress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
