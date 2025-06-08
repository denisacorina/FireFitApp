using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

public interface IUpdateUserContext
{
    Task<bool> Execute(string userId, string email, string name, int age, bool isMale, int height, decimal startingWeight, decimal targetWeight, WeightChangeType changeType, ActivityLevel activityLevel, List<DietaryPreference> dietaryPreferences, List<WorkoutType> workoutTypes, string profilePicturePath, ExperienceLevel fitnessExperience);
}
