using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Application.DTOs
{
    public class WorkoutSessionDto
    {
        public Guid SessionId { get; set; }
        public string UserId { get; set; }
        public WorkoutType WorkoutType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
        public int IntensityLevel { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public string? Notes { get; set; }
    }
}