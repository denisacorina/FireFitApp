
using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class UserPreferences
    {
        [Key]
        public string UserId { get; set; }
        public List<DietaryPreference> DietaryPreferences { get; set; } = new();
        public bool IsVegan => DietaryPreferences.Contains(DietaryPreference.Vegan);
        public bool IsGlutenFree => DietaryPreferences.Contains(DietaryPreference.GlutenFree);
        public bool IsLactoseIntolerant => DietaryPreferences.Contains(DietaryPreference.LactoseIntolerant);
        public int DailyCalorieGoal { get; set; }

        private UserPreferences() { }

        public static UserPreferences Create(string userId, List<DietaryPreference> dietaryPreferences, int dailyCalorieGoal)
        {
            return new UserPreferences
            {
                UserId = userId,
                DietaryPreferences = dietaryPreferences ?? new List<DietaryPreference>(),
                DailyCalorieGoal = dailyCalorieGoal
            };
        }

        public UserPreferences Update(List<DietaryPreference> dietaryPreferences, int newCalorieGoal)
        {
            return new UserPreferences
            {
                UserId = this.UserId,
                DietaryPreferences = dietaryPreferences ?? new List<DietaryPreference>(),
                DailyCalorieGoal = newCalorieGoal
            };
        }
    }
}
