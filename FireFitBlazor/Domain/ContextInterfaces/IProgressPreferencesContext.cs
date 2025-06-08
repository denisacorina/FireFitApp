using FireFitBlazor.Domain.Models;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.ContextInterfaces
{
    public interface IUserProgressContext
    {
        Task<Result<UserProgress?>> GetUserProgressAsync(string userId);
        Task<UserProgress> CreateProgressAsync(string userId, decimal startingWeight, decimal? startingBodyFat = null);
        Task UpdateWeightAsync(string userId, decimal newWeight, string? notes = null);
        Task AddMeasurementAsync(string userId, BodyMeasurement measurement);
        Task AddWorkoutSessionAsync(string userId, WorkoutSession session);
    }

    public interface IUserPreferencesContext
    {
        Task<bool> UpdateDietaryPreferencesAsync(string userId, IEnumerable<DietaryPreference> preferences);
        Task<bool> UpdateWorkoutPreferencesAsync(string userId, List<WorkoutPreference> selectedTypes);
        Task<bool> UpdateUserPreferencesAsync(string userId, List<DietaryPreference> dietaryPreferences, int dailyCalorieGoal);
    }
}
