using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireFitBlazor.Migrations
{
    /// <inheritdoc />
    public partial class NewItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ARPortionControl",
                columns: table => new
                {
                    PortionControlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FoodName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuggestedPortionSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARPortionControl", x => x.PortionControlId);
                });

            migrationBuilder.CreateTable(
                name: "BarcodeProducts",
                columns: table => new
                {
                    BarcodeNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Proteins = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Carbs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fats = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeProducts", x => x.BarcodeNumber);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseLog",
                columns: table => new
                {
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExerciseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    CaloriesBurned = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseLog", x => x.ExerciseId);
                });

            migrationBuilder.CreateTable(
                name: "FoodLogs",
                columns: table => new
                {
                    FoodLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FoodName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Proteins = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Carbs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fats = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodLogs", x => x.FoodLogId);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    GoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Proteins = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Carbs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fats = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IntermittentFasting = table.Column<bool>(type: "bit", nullable: false),
                    FastingWindowHours = table.Column<int>(type: "int", nullable: false),
                    DietaryPreference = table.Column<int>(type: "int", nullable: false),
                    TargetWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TargetBodyFatPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.GoalId);
                });

            migrationBuilder.CreateTable(
                name: "IngredientRecognition",
                columns: table => new
                {
                    RecognitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientRecognition", x => x.RecognitionId);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.MealId);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Proteins = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Carbs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fats = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.RecipeId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    TargetWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeightChangeType = table.Column<int>(type: "int", nullable: false),
                    ActivityLevel = table.Column<int>(type: "int", nullable: false),
                    DietaryPreferences = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FitnessExperience = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "MealIngredient",
                columns: table => new
                {
                    MealIngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuantityGrams = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealIngredient", x => x.MealIngredientId);
                    table.ForeignKey(
                        name: "FK_MealIngredient_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "MealId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient",
                columns: table => new
                {
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Proteins = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Carbs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fats = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContainsAnimalProducts = table.Column<bool>(type: "bit", nullable: false),
                    ContainsGluten = table.Column<bool>(type: "bit", nullable: false),
                    ContainsLactose = table.Column<bool>(type: "bit", nullable: false),
                    IngredientRecognitionRecognitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => x.IngredientId);
                    table.ForeignKey(
                        name: "FK_Ingredient_IngredientRecognition_IngredientRecognitionRecognitionId",
                        column: x => x.IngredientRecognitionRecognitionId,
                        principalTable: "IngredientRecognition",
                        principalColumn: "RecognitionId");
                    table.ForeignKey(
                        name: "FK_Ingredient_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId");
                });

            migrationBuilder.CreateTable(
                name: "CalorieLog",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedCalories = table.Column<int>(type: "int", nullable: false),
                    RecommendedCalories = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalorieLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_CalorieLog_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DietaryPreferences = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DailyCalorieGoal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProgress",
                columns: table => new
                {
                    ProgressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartingWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartingBodyFatPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentBodyFatPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WeightChange = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BodyFatChange = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMeasurementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProgress", x => x.ProgressId);
                    table.ForeignKey(
                        name: "FK_UserProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutPreferences",
                columns: table => new
                {
                    PreferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PreferredDay = table.Column<int>(type: "int", nullable: false),
                    PreferredTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    IntensityLevel = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPreferences", x => x.PreferenceId);
                    table.ForeignKey(
                        name: "FK_WorkoutPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    AchievementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserProgressProgressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.AchievementId);
                    table.ForeignKey(
                        name: "FK_Achievements_UserProgress_UserProgressProgressId",
                        column: x => x.UserProgressProgressId,
                        principalTable: "UserProgress",
                        principalColumn: "ProgressId");
                });

            migrationBuilder.CreateTable(
                name: "BodyMeasurements",
                columns: table => new
                {
                    MeasurementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeasurementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BodyFatPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Chest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Waist = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Hips = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LeftArm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RightArm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LeftThigh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RightThigh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LeftCalf = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RightCalf = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserProgressProgressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyMeasurements", x => x.MeasurementId);
                    table.ForeignKey(
                        name: "FK_BodyMeasurements_UserProgress_UserProgressProgressId",
                        column: x => x.UserProgressProgressId,
                        principalTable: "UserProgress",
                        principalColumn: "ProgressId");
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSession",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkoutType = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    CaloriesBurned = table.Column<int>(type: "int", nullable: false),
                    IntensityLevel = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    UserProgressProgressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSession", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_WorkoutSession_UserProgress_UserProgressProgressId",
                        column: x => x.UserProgressProgressId,
                        principalTable: "UserProgress",
                        principalColumn: "ProgressId");
                });

            migrationBuilder.CreateTable(
                name: "Exercise",
                columns: table => new
                {
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sets = table.Column<int>(type: "int", nullable: false),
                    Reps = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    CaloriesBurned = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkoutSessionSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercise", x => x.ExerciseId);
                    table.ForeignKey(
                        name: "FK_Exercise_WorkoutSession_WorkoutSessionSessionId",
                        column: x => x.WorkoutSessionSessionId,
                        principalTable: "WorkoutSession",
                        principalColumn: "SessionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_UserProgressProgressId",
                table: "Achievements",
                column: "UserProgressProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_BodyMeasurements_UserProgressProgressId",
                table: "BodyMeasurements",
                column: "UserProgressProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_CalorieLog_UserId",
                table: "CalorieLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_WorkoutSessionSessionId",
                table: "Exercise",
                column: "WorkoutSessionSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_IngredientRecognitionRecognitionId",
                table: "Ingredient",
                column: "IngredientRecognitionRecognitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_RecipeId",
                table: "Ingredient",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_MealIngredient_MealId",
                table: "MealIngredient",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_UserId",
                table: "UserProgress",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPreferences_UserId",
                table: "WorkoutPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSession_UserProgressProgressId",
                table: "WorkoutSession",
                column: "UserProgressProgressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "ARPortionControl");

            migrationBuilder.DropTable(
                name: "BarcodeProducts");

            migrationBuilder.DropTable(
                name: "BodyMeasurements");

            migrationBuilder.DropTable(
                name: "CalorieLog");

            migrationBuilder.DropTable(
                name: "Exercise");

            migrationBuilder.DropTable(
                name: "ExerciseLog");

            migrationBuilder.DropTable(
                name: "FoodLogs");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "Ingredient");

            migrationBuilder.DropTable(
                name: "MealIngredient");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "WorkoutPreferences");

            migrationBuilder.DropTable(
                name: "WorkoutSession");

            migrationBuilder.DropTable(
                name: "IngredientRecognition");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "UserProgress");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
