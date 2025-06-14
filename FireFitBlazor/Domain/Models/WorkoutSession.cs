using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class WorkoutSession
    {
        [Key]
        public Guid SessionId { get; private init; }
        public string UserId { get; private init; }
        public WorkoutType WorkoutType { get; private init; }
        public DateTime StartTime { get; private init; }
        public DateTime? EndTime { get; private init; }
        public int DurationMinutes { get; private init; }
        public int CaloriesBurned { get; private init; }
        public int IntensityLevel { get; private init; } // 1-10
        public string? Notes { get; private init; }
        public int Sets { get; private init; }
        public int Reps { get; private init; }
        public bool IsCompleted { get; private init; }

        private WorkoutSession() { }

        private WorkoutSession(
            Guid sessionId,
            string userId,
            WorkoutType workoutType,
            DateTime startTime,
            DateTime? endTime,
            int durationMinutes,
            int caloriesBurned,
            int intensityLevel,
            string? notes,
            int? sets,
            int? reps,
            bool isCompleted)
        {
            SessionId = sessionId;
            UserId = userId;
            WorkoutType = workoutType;
            StartTime = startTime;
            EndTime = endTime;
            DurationMinutes = durationMinutes;
            CaloriesBurned = caloriesBurned;
            IntensityLevel = intensityLevel;
            Notes = notes;
            Sets = sets ?? 0;
            Reps = reps ?? 0;
            IsCompleted = isCompleted;
        }

        public static WorkoutSession Create(
            string userId,
            WorkoutType workoutType,
            DateTime startTime,
            DateTime endTime,
            int durationMinutes,
            int caloriesBurned,
            int intensityLevel,
            string? notes = null)
        {
            return new WorkoutSession(
                Guid.NewGuid(),
                userId,
                workoutType,
                startTime,
                null,
                durationMinutes,
                caloriesBurned,
                Math.Clamp(intensityLevel, 1, 10),
                notes,
                0,
                0,
                false
            );
        }


        public WorkoutSession Update(
    WorkoutType? workoutType = null,
    DateTime? startTime = null,
    DateTime? endTime = null,
    int? durationMinutes = null,
    int? caloriesBurned = null,
    int? intensityLevel = null,
    string? notes = null,
    bool? isCompleted = null,
    int? sets = null,
    int? reps = null)
        {
            return new WorkoutSession
            {
                SessionId = this.SessionId,
                UserId = this.UserId,
                WorkoutType = workoutType ?? this.WorkoutType,
                StartTime = startTime ?? this.StartTime,
                EndTime = endTime ?? this.EndTime,
                DurationMinutes = durationMinutes ?? this.DurationMinutes,
                CaloriesBurned = caloriesBurned ?? this.CaloriesBurned,
                IntensityLevel = intensityLevel.HasValue ? Math.Clamp(intensityLevel.Value, 1, 10) : this.IntensityLevel,
                Notes = notes ?? this.Notes,
                IsCompleted = isCompleted ?? this.IsCompleted,
                Sets = sets ?? this.Sets,
                Reps = reps ?? this.Reps
            };
        }

     
    }
}