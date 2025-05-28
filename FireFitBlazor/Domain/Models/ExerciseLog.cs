
using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.Models
{
    public sealed class ExerciseLog
    {
        [Key]
        public Guid ExerciseId { get; set; }
        public string UserId { get; set; }
        public string ExerciseName { get; set; }
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
        public DateTime Timestamp { get; set; }

        private ExerciseLog() { }

        public static ExerciseLog Create(string userId, string exerciseName, int duration, int caloriesBurned)
        {
            return new ExerciseLog
            {
                ExerciseId = Guid.NewGuid(),
                UserId = userId,
                ExerciseName = exerciseName,
                DurationMinutes = duration,
                CaloriesBurned = caloriesBurned,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
