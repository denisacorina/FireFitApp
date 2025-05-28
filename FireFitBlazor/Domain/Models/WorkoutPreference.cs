using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class WorkoutPreference
    {
        [Key]
        public Guid PreferenceId { get; set; }
        public string UserId { get; set; }
        public WorkoutType Type { get; set; }
        public DayOfWeek PreferredDay { get; set; }
        public TimeSpan PreferredTime { get; set; }
        public int DurationMinutes { get; set; }
        public int IntensityLevel { get; set; } // 1-10
        public bool IsEnabled { get; set; }

        public static WorkoutPreference Create(
            string userId,
            WorkoutType type,
            DayOfWeek preferredDay,
            TimeSpan preferredTime,
            int durationMinutes,
            int intensityLevel)
        {
            return new WorkoutPreference
            {
                PreferenceId = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                PreferredDay = preferredDay,
                PreferredTime = preferredTime,
                DurationMinutes = durationMinutes,
                IntensityLevel = Math.Clamp(intensityLevel, 1, 10),
                IsEnabled = true
            };
        }
    }
}
