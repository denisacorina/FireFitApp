namespace FireFitBlazor.Application.Services;

using FireFitBlazor.Domain.Models;

public interface IFoodLogService
{
    Task ImportIngredientsSimple(string csvPath);
    Task<List<FoodLog>> SearchFoodsAsync(string query);
    Task<List<string>> GetAllIngredientNames();
    Task LogFoodAsync(FoodLog item);
    Task<List<FoodLog>> GetLogsForDate(string userId, DateTime date);
    Task<List<Ingredient>> GetIngredientDetails(List<string> names);
    Task SaveFoodLogAsync(FoodLog log);
    Task<int> GetDailyGoalCalories(string userId);   

    Task SaveMealAsync(string mealName, string userId, List<DetectedIngredient> ingredients);

    Task<NutritionalSummary> GetMacrosForIngredients(List<string> ingredientNames);
    
}

