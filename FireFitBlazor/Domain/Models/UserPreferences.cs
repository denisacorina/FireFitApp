using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;
using System.ComponentModel.DataAnnotations;

public sealed class UserPreferences
{
    [Key]
    public string UserId { get; init;  }

    public IReadOnlyList<DietaryPreference> DietaryPreferences { get; init;  }
    public bool IsVegan => DietaryPreferences.Contains(DietaryPreference.Vegan);
    public bool IsGlutenFree => DietaryPreferences.Contains(DietaryPreference.GlutenFree);
    public bool IsLactoseIntolerant => DietaryPreferences.Contains(DietaryPreference.LactoseIntolerant);
    public int DailyCalorieGoal { get; }

    private UserPreferences() { }

    private UserPreferences(
          string userId,
          IReadOnlyList<DietaryPreference> dietaryPreferences,
          int dailyCalorieGoal)
    {
        UserId = userId;
        DietaryPreferences = dietaryPreferences;
        DailyCalorieGoal = dailyCalorieGoal;
    }

    public static UserPreferences Create(
        string userId,
        List<DietaryPreference>? dietaryPreferences,
        int dailyCalorieGoal)
    {
        return new UserPreferences(
            userId: userId,
            dietaryPreferences: dietaryPreferences ?? new List<DietaryPreference>(),
            dailyCalorieGoal: dailyCalorieGoal
        );
    }

    public UserPreferences Update(
        List<DietaryPreference>? dietaryPreferences,
        int newCalorieGoal)
    {
        return new UserPreferences(
            userId: this.UserId,
            dietaryPreferences: dietaryPreferences ?? new List<DietaryPreference>(),
            dailyCalorieGoal: newCalorieGoal
        );
    }
}
