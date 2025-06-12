using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireFitBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddedWorkoutSessionsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_WorkoutSession_WorkoutSessionSessionId",
                table: "Exercise");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSession_UserProgress_UserProgressProgressId",
                table: "WorkoutSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutSession",
                table: "WorkoutSession");

            migrationBuilder.RenameTable(
                name: "WorkoutSession",
                newName: "WorkoutSessions");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutSession_UserProgressProgressId",
                table: "WorkoutSessions",
                newName: "IX_WorkoutSessions_UserProgressProgressId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "WorkoutSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_WorkoutSessions_WorkoutSessionSessionId",
                table: "Exercise",
                column: "WorkoutSessionSessionId",
                principalTable: "WorkoutSessions",
                principalColumn: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessions_UserProgress_UserProgressProgressId",
                table: "WorkoutSessions",
                column: "UserProgressProgressId",
                principalTable: "UserProgress",
                principalColumn: "ProgressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_WorkoutSessions_WorkoutSessionSessionId",
                table: "Exercise");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_UserProgress_UserProgressProgressId",
                table: "WorkoutSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "WorkoutSessions");

            migrationBuilder.RenameTable(
                name: "WorkoutSessions",
                newName: "WorkoutSession");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutSessions_UserProgressProgressId",
                table: "WorkoutSession",
                newName: "IX_WorkoutSession_UserProgressProgressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutSession",
                table: "WorkoutSession",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_WorkoutSession_WorkoutSessionSessionId",
                table: "Exercise",
                column: "WorkoutSessionSessionId",
                principalTable: "WorkoutSession",
                principalColumn: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSession_UserProgress_UserProgressProgressId",
                table: "WorkoutSession",
                column: "UserProgressProgressId",
                principalTable: "UserProgress",
                principalColumn: "ProgressId");
        }
    }
}
