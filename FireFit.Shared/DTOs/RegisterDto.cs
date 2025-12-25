using FireFit.Shared.Enums;

namespace FireFit.Shared.DTOs;

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public int Height { get; set; }
    public decimal StartingWeight { get; set; }
    public decimal CurrentWeight { get; set; }
    public decimal TargetWeight { get; set; }
    public Gender Gender { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
    public WeightChangeType WeightGoal { get; set; }
    public IEnumerable<DietaryPreference> DietaryPreferences { get; set; } = Array.Empty<DietaryPreference>();
    public IEnumerable<WorkoutType> WorkoutTypes { get; set; } = Array.Empty<WorkoutType>();
    public int DailyCalorieGoal { get; set; }
    public int ProteinGoal { get; set; }
    public int CarbsGoal { get; set; }
    public int FatsGoal { get; set; }
    public ExperienceLevel FitnessExperience { get; set; }
}

