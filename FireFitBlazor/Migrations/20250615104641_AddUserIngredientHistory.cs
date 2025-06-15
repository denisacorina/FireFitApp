using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireFitBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIngredientHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserIngredientHistory",
                columns: table => new
                {
                    HistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UseCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIngredientHistory", x => x.HistoryId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserIngredientHistory");
        }
    }
}
