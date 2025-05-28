namespace FireFitBlazor.Application.Services;

using FireFitBlazor.Domain.Models;

public interface IFoodLogService
{
    Task<List<FoodLog>> SearchFoodsAsync(string query);
    Task<List<string>> GetAllIngredientNames();
    Task LogFoodAsync(FoodLog item);
    Task<List<FoodLog>> GetLogsForDate(string userId, DateTime date);
    Task<List<Ingredient>> GetIngredientDetails(List<string> names);
    Task SaveFoodLogAsync(FoodLog log);
    Task<int> GetDailyGoalCalories(string userId);   

    Task SaveMealAsync(string mealName, string userId, List<DetectedIngredient> ingredients);


    //Task<List<DetectedIngredient>> DetectIngredientsFromImage(string base64);
    Task<NutritionalSummary> GetMacrosForIngredients(List<string> ingredientNames);
    
}

