using FireFit.Shared.Enums;

namespace FireFit.Shared.DTOs;

public class ProfileUpdateDto
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public int Height { get; set; }
    public decimal StartingWeight { get; set; }
    public decimal TargetWeight { get; set; }
    public WeightChangeType WeightGoal { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
    public ExperienceLevel FitnessExperience { get; set; }
    public IList<DietaryPreference> DietaryPreferences { get; set; } = new List<DietaryPreference>();
    public IList<WorkoutType> WorkoutTypes { get; set; } = new List<WorkoutType>();
    public IList<WorkoutPreferenceDto> WorkoutPreferences { get; set; } = new List<WorkoutPreferenceDto>();
    public string? ProfilePicturePath { get; set; }
}
