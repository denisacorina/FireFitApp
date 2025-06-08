using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class UserProgress
    {
        [Key]
        public Guid ProgressId { get; init;  }
        public string UserId { get; init; }

        // Existing weight and body fat tracking
        public decimal StartingWeight { get; init; }
        public decimal CurrentWeight { get; init; }
        public decimal? StartingBodyFatPercentage { get; init; }
        public decimal? CurrentBodyFatPercentage { get; init; }
        public decimal WeightChange { get; init; }
        public decimal? BodyFatChange { get; init; }

        // Timestamps
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public DateTime? LastMeasurementDate { get; init; }
        public string? Notes { get; init; }

        // New tracking properties
        public List<BodyMeasurement> Measurements { get; init; } = new();
        public List<WorkoutSession> WorkoutSessions { get; init; } = new();
        public List<Achievement> Achievements { get; init; } = new();

        private UserProgress() { }
        private UserProgress(
         Guid progressId,
         string userId,
         decimal startingWeight,
         decimal currentWeight,
         decimal? startingBodyFatPercentage,
         decimal? currentBodyFatPercentage,
         decimal weightChange,
         decimal? bodyFatChange,
         DateTime createdAt,
         DateTime updatedAt,
         DateTime? lastMeasurementDate,
         string? notes,
         List<BodyMeasurement> measurements,
         List<WorkoutSession> workoutSessions,
         List<Achievement> achievements)
        {
            ProgressId = progressId;
            UserId = userId;
            StartingWeight = startingWeight;
            CurrentWeight = currentWeight;
            StartingBodyFatPercentage = startingBodyFatPercentage;
            CurrentBodyFatPercentage = currentBodyFatPercentage;
            WeightChange = weightChange;
            BodyFatChange = bodyFatChange;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            LastMeasurementDate = lastMeasurementDate;
            Notes = notes;
            Measurements = measurements;
            WorkoutSessions = workoutSessions;
            Achievements = achievements;
        }

        public static UserProgress Create(
          string userId,
          decimal startingWeight,
          decimal currentWeight,
          decimal? startingBodyFatPercentage = null,
          decimal? currentBodyFatPercentage = null,
          string? notes = null)
        {
            var now = DateTime.UtcNow;
            return new UserProgress(
                progressId: Guid.NewGuid(),
                userId: userId,
                startingWeight: startingWeight,
                currentWeight: currentWeight,
                startingBodyFatPercentage: startingBodyFatPercentage,
                currentBodyFatPercentage: currentBodyFatPercentage,
                weightChange: startingWeight - currentWeight,
                bodyFatChange: startingBodyFatPercentage - currentBodyFatPercentage,
                createdAt: now,
                updatedAt: now,
                lastMeasurementDate: now,
                notes: notes,
                measurements: new List<BodyMeasurement>(),
                workoutSessions: new List<WorkoutSession>(),
                achievements: new List<Achievement>()
            );
        }

        public UserProgress Update(
    decimal? currentWeight = null,
    decimal? currentBodyFatPercentage = null,
    string? notes = null,
    List<BodyMeasurement>? measurements = null,
    List<WorkoutSession>? workoutSessions = null,
    List<Achievement>? achievements = null)
        {
            return With(
                currentWeight: currentWeight,
                currentBodyFatPercentage: currentBodyFatPercentage,
                notes: notes,
                measurements: measurements,
                workoutSessions: workoutSessions,
                achievements: achievements
            );
        }

        private UserProgress With(
            decimal? currentWeight = null,
            decimal? currentBodyFatPercentage = null,
            string? notes = null,
            List<BodyMeasurement>? measurements = null,
            List<WorkoutSession>? workoutSessions = null,
            List<Achievement>? achievements = null)
        {
            var now = DateTime.UtcNow;
            return new UserProgress(
                progressId: ProgressId,
                userId: UserId,
                startingWeight: StartingWeight,
                currentWeight: currentWeight ?? CurrentWeight,
                startingBodyFatPercentage: StartingBodyFatPercentage ?? currentBodyFatPercentage,
                currentBodyFatPercentage: currentBodyFatPercentage ?? CurrentBodyFatPercentage,
                weightChange: StartingWeight - (currentWeight ?? CurrentWeight),
                bodyFatChange: (StartingBodyFatPercentage ?? currentBodyFatPercentage) - (currentBodyFatPercentage ?? CurrentBodyFatPercentage),
                createdAt: CreatedAt,
                updatedAt: now,
                lastMeasurementDate: now,
                notes: notes ?? Notes,
                measurements: measurements ?? Measurements,
                workoutSessions: workoutSessions ?? WorkoutSessions,
                achievements: achievements ?? Achievements
            );
        }

        public UserProgress UpdateWeight(decimal newWeight, string? notes = null)
            => With(currentWeight: newWeight, notes: notes);

        public UserProgress UpdateBodyFat(decimal newBodyFatPercentage, string? notes = null)
            => With(currentBodyFatPercentage: newBodyFatPercentage, notes: notes);

        public UserProgress UpdateNotes(string notes)
            => With(notes: notes);

        public UserProgress AddMeasurement(BodyMeasurement measurement)
        {
            var updatedMeasurements = new List<BodyMeasurement>(Measurements) { measurement };
            return With(measurements: updatedMeasurements);
        }

        public UserProgress AddWorkoutSession(WorkoutSession session)
        {
            var updatedSessions = new List<WorkoutSession>(WorkoutSessions) { session };
            return With(workoutSessions: updatedSessions);
        }

        public UserProgress AddAchievement(Achievement achievement)
        {
            var updatedAchievements = new List<Achievement>(Achievements) { achievement };
            return With(achievements: updatedAchievements);
        }

        public int GetTotalCaloriesBurned(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = WorkoutSessions.Where(s => s.IsCompleted);

            if (startDate.HasValue)
                query = query.Where(s => s.StartTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.EndTime <= endDate.Value);

            return query.Sum(s => s.CaloriesBurned);
        }

        public Dictionary<WorkoutType, int> GetWorkoutStats(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = WorkoutSessions.Where(s => s.IsCompleted);

            if (startDate.HasValue)
                query = query.Where(s => s.StartTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.EndTime <= endDate.Value);

            return query
                .GroupBy(s => s.WorkoutType)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

        public List<BodyMeasurement> GetMeasurementHistory(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = Measurements.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(m => m.MeasurementDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.MeasurementDate <= endDate.Value);

            return query.OrderByDescending(m => m.MeasurementDate).ToList();
        }

        public List<Achievement> GetRecentAchievements(int count = 5)
        {
            return Achievements
                .OrderByDescending(a => a.EarnedAt)
                .Take(count)
                .ToList();
        }
    }
}
