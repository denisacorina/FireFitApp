namespace FireFitBlazor.Application.Services;

using FireFitBlazor.Domain.Models;

public interface IFoodLogService
{
    Task ImportIngredientsSimple(string csvPath);
    Task<List<FoodLog>> SearchFoodsAsync(string query);
    Task<List<Ingredient>> GetAllIngredientNames();
    //Task LogFoodAsync(FoodLog item);
    Task<List<FoodLog>> GetLogsForDate(string userId, DateTime date);
    Task<List<Ingredient>> GetIngredientDetails(List<string> names);
    Task<List<Ingredient>> GetIngredientDetailsById(List<Guid> ingredientIds);
    Task<Ingredient> GetIngredientDetails(Guid ingredientId);
    Task SaveFoodLogAsync(FoodLog log);
    Task<int> GetDailyGoalCalories(string userId);   

    Task SaveMealAsync(string mealName, string userId, List<DetectedIngredient> ingredients);

    Task<NutritionalSummary> GetMacrosForIngredients(List<string> ingredientNames);
    Task DeleteLog(Guid logId);
    Task<FoodLog?> GetLogById(Guid logId);
    Task UpdateFoodLogAsync(FoodLog updatedLog);
    Task<List<FoodLog>> GetRecentByUserIdAsync(string userId, int days);
    
    // User Ingredient History methods
    Task<List<UserIngredientHistory>> GetUserIngredientHistoryAsync(string userId, int limit = 20);
    Task<UserIngredientHistory> AddToUserHistoryAsync(string userId, Guid ingredientId, string ingredientName);
    Task<UserIngredientHistory> UpdateIngredientUsageAsync(Guid historyId);
}

