using System;
using System.Linq;
using FireFitBlazor.Domain.Enums;
using FireFit.Shared.DTOs;
using DomainEnums = FireFitBlazor.Domain.Enums;
using DomainModels = FireFitBlazor.Domain.Models;
using SharedEnums = FireFit.Shared.Enums;

namespace FireFitBlazor.Infrastructure.Adapters;

internal static class SharedMappings
{
    public static SharedEnums.MealType ToShared(this DomainEnums.FoodTrackingEnums.MealType v) => Enum.Parse<SharedEnums.MealType>(v.ToString());
    public static DomainEnums.FoodTrackingEnums.MealType ToDomain(this SharedEnums.MealType v) => Enum.Parse<DomainEnums.FoodTrackingEnums.MealType>(v.ToString());

    public static SharedEnums.DietaryPreference ToShared(this DomainEnums.FoodTrackingEnums.DietaryPreference v) => Enum.Parse<SharedEnums.DietaryPreference>(v.ToString());
    public static DomainEnums.FoodTrackingEnums.DietaryPreference ToDomain(this SharedEnums.DietaryPreference v) => Enum.Parse<DomainEnums.FoodTrackingEnums.DietaryPreference>(v.ToString());

    public static SharedEnums.WeightChangeType ToShared(this DomainEnums.FoodTrackingEnums.WeightChangeType v) => Enum.Parse<SharedEnums.WeightChangeType>(v.ToString());
    public static DomainEnums.FoodTrackingEnums.WeightChangeType ToDomain(this SharedEnums.WeightChangeType v) => Enum.Parse<DomainEnums.FoodTrackingEnums.WeightChangeType>(v.ToString());

    public static SharedEnums.ActivityLevel ToShared(this DomainEnums.FoodTrackingEnums.ActivityLevel v) => Enum.Parse<SharedEnums.ActivityLevel>(v.ToString());
    public static DomainEnums.FoodTrackingEnums.ActivityLevel ToDomain(this SharedEnums.ActivityLevel v) => Enum.Parse<DomainEnums.FoodTrackingEnums.ActivityLevel>(v.ToString());

    public static SharedEnums.Gender ToShared(this DomainEnums.FoodTrackingEnums.Gender v) => Enum.Parse<SharedEnums.Gender>(v.ToString());
    public static DomainEnums.FoodTrackingEnums.Gender ToDomain(this SharedEnums.Gender v) => Enum.Parse<DomainEnums.FoodTrackingEnums.Gender>(v.ToString());

    public static SharedEnums.ExperienceLevel ToShared(this DomainEnums.FoodTrackingEnums.ExperienceLevel v) => Enum.Parse<SharedEnums.ExperienceLevel>(v.ToString());
    public static DomainEnums.FoodTrackingEnums.ExperienceLevel ToDomain(this SharedEnums.ExperienceLevel v) => Enum.Parse<DomainEnums.FoodTrackingEnums.ExperienceLevel>(v.ToString());

    public static SharedEnums.WorkoutType ToShared(this DomainEnums.FoodTrackingEnums.WorkoutType v) => Enum.Parse<SharedEnums.WorkoutType>(v.ToString());
    public static DomainEnums.FoodTrackingEnums.WorkoutType ToDomain(this SharedEnums.WorkoutType v) => Enum.Parse<DomainEnums.FoodTrackingEnums.WorkoutType>(v.ToString());

    public static SharedEnums.GoalType ToShared(this DomainEnums.GoalType v) => Enum.Parse<SharedEnums.GoalType>(v.ToString());
    public static DomainEnums.GoalType ToDomain(this SharedEnums.GoalType v) => Enum.Parse<DomainEnums.GoalType>(v.ToString());

    public static UserDto ToShared(this DomainModels.User u)
    {
        return new UserDto(
            u.UserId,  
            u.Email,
            u.Name,
            u.Age,
            u.Gender == DomainEnums.FoodTrackingEnums.Gender.Male,
            u.Height,
            u.StartingWeight,
            u.TargetWeight,
            u.WeightGoal.ChangeType.ToShared(),
            u.ActivityLevel.ToShared(),
            u.DietaryPreferences.Select(ToShared),
            u.WorkoutTypes.Select(ToShared),
            0,
            0,
            0,
            0,
            u.FitnessExperience.ToShared(),
            u.ProfilePicturePath,
            u.WorkoutPreferences.Select(p => p.ToShared())
        );
    }

    public static WorkoutPreferenceDto ToShared(this DomainModels.WorkoutPreference preference)
        => new WorkoutPreferenceDto
        {
            Type = preference.Type.ToShared(),
            PreferredDay = preference.PreferredDay,
            PreferredTime = preference.PreferredTime,
            DurationMinutes = preference.DurationMinutes,
            IntensityLevel = preference.IntensityLevel,
            IsEnabled = preference.IsEnabled
        };

    public static WorkoutSessionDto ToShared(this DomainModels.WorkoutSession session)
        => new WorkoutSessionDto
        {
            SessionId = session.SessionId,
            UserId = session.UserId,
            WorkoutType = session.WorkoutType.ToShared(),
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            DurationMinutes = session.DurationMinutes,
            CaloriesBurned = session.CaloriesBurned,
            IntensityLevel = session.IntensityLevel,
            Notes = session.Notes,
            Sets = session.Sets,
            Reps = session.Reps
        };

    public static FoodLogDto ToShared(this DomainModels.FoodLog f)
    {
        return new FoodLogDto
        {
            Id = f.FoodLogId.ToString(),
            UserId = f.UserId,
            FoodName = f.FoodName,
            Calories = (int)f.NutritionalInfo.Calories,
            Proteins = f.NutritionalInfo.Proteins,
            Carbs = f.NutritionalInfo.Carbs,
            Fats = f.NutritionalInfo.Fats,
            MealType = f.MealType.HasValue ? f.MealType.Value.ToShared() : default,
            TimestampUtc = f.Timestamp
        };
    }

    public static DomainModels.FoodLog ToDomain(this FoodLogDto f)
    {
        var meal = f.MealType.ToDomain();
        return DomainModels.FoodLog.Create(
            f.UserId,
            f.FoodName,
            f.Calories,
            f.Proteins,
            f.Carbs,
            f.Fats,
            meal
        );
    }
}
