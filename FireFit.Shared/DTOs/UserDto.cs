using FireFit.Shared.Enums;

namespace FireFit.Shared.DTOs;

public record UserDto(
    string UserId,
    string Email,
    string Name,
    int Age,
    bool IsMale,
    int Height,
    decimal StartingWeight,
    decimal TargetWeight,
    WeightChangeType WeightGoal,
    ActivityLevel ActivityLevel,
    IEnumerable<DietaryPreference> DietaryPreferences,
    IEnumerable<WorkoutType> WorkoutTypes,
    int DailyCalorieGoal,
    float ProteinGoal,
    float CarbsGoal,
    float FatsGoal,
    ExperienceLevel FitnessExperience,
    string? ProfilePicturePath,
    IEnumerable<WorkoutPreferenceDto> WorkoutPreferences
);
