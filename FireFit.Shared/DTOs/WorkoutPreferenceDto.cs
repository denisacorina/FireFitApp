using FireFit.Shared.Enums;

namespace FireFit.Shared.DTOs;

public class WorkoutPreferenceDto
{
    public WorkoutType Type { get; set; }
    public DayOfWeek PreferredDay { get; set; } = DayOfWeek.Monday;
    public TimeSpan PreferredTime { get; set; } = TimeSpan.FromHours(9);
    public int DurationMinutes { get; set; } = 60;
    public int IntensityLevel { get; set; } = 5;
    public bool IsEnabled { get; set; } = true;
}
