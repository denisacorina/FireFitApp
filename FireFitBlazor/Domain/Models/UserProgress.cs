using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class UserProgress
    {
        [Key]
        public Guid ProgressId { get; set; }
        public string UserId { get; set; }

        // Existing weight and body fat tracking
        public decimal StartingWeight { get; set; }
        public decimal CurrentWeight { get; set; }
        public decimal? StartingBodyFatPercentage { get; set; }
        public decimal? CurrentBodyFatPercentage { get; set; }
        public decimal WeightChange { get; set; }
        public decimal? BodyFatChange { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastMeasurementDate { get; set; }
        public string? Notes { get; set; }

        // New tracking properties
        public List<BodyMeasurement> Measurements { get; set; } = new();
        public List<WorkoutSession> WorkoutSessions { get; set; } = new();
        public List<Achievement> Achievements { get; set; } = new();

        private UserProgress() { }

        public static UserProgress Create(
            string userId,
            decimal startingWeight,
            decimal currentWeight,
            decimal? startingBodyFatPercentage = null,
            decimal? currentBodyFatPercentage = null,
            string? notes = null)
        {
            return new UserProgress
            {
                ProgressId = Guid.NewGuid(),
                UserId = userId,
                StartingWeight = startingWeight,
                CurrentWeight = currentWeight,
                StartingBodyFatPercentage = startingBodyFatPercentage,
                CurrentBodyFatPercentage = currentBodyFatPercentage,
                WeightChange = startingWeight - currentWeight,
                BodyFatChange = startingBodyFatPercentage - currentBodyFatPercentage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastMeasurementDate = DateTime.UtcNow,
                Notes = notes
            };
        }

        // Existing methods
        public void UpdateWeight(decimal newWeight, string? notes = null)
        {
            WeightChange = StartingWeight - newWeight;
            CurrentWeight = newWeight;
            UpdateLastModified();
            Notes = notes ?? Notes;
        }

        public void UpdateBodyFat(decimal newBodyFatPercentage, string? notes = null)
        {
            BodyFatChange = StartingBodyFatPercentage - newBodyFatPercentage;
            CurrentBodyFatPercentage = newBodyFatPercentage;
            UpdateLastModified();
            Notes = notes ?? Notes;
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes;
            UpdateLastModified();
        }

        // New methods for tracking
        public void AddMeasurement(BodyMeasurement measurement)
        {
            Measurements.Add(measurement);
            UpdateLastModified();
        }

        public void AddWorkoutSession(WorkoutSession session)
        {
            WorkoutSessions.Add(session);
            UpdateLastModified();
        }

        public void AddAchievement(Achievement achievement)
        {
            Achievements.Add(achievement);
            UpdateLastModified();
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

        private void UpdateLastModified()
        {
            UpdatedAt = DateTime.UtcNow;
            LastMeasurementDate = DateTime.UtcNow;
        }
    }
}
