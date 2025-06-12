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
        public DateTime Date { get; private init; }
        public int DurationMinutes { get; private init; }
        public int CaloriesBurned { get; private init; }
        public int IntensityLevel { get; private init; } // 1-10
        public string? Notes { get; private init; }
        public IReadOnlyList<Exercise> Exercises { get; private init; } = new List<Exercise>();
        public bool IsCompleted { get; private init; }

        private WorkoutSession() { }

        private WorkoutSession(
            Guid sessionId,
            string userId,
            WorkoutType workoutType,
            DateTime startTime,
            DateTime? endTime,
            DateTime date,
            int durationMinutes,
            int caloriesBurned,
            int intensityLevel,
            string? notes,
            IReadOnlyList<Exercise> exercises,
            bool isCompleted)
        {
            SessionId = sessionId;
            UserId = userId;
            WorkoutType = workoutType;
            StartTime = startTime;
            EndTime = endTime;
            Date = DateTime.UtcNow;
            DurationMinutes = durationMinutes;
            CaloriesBurned = caloriesBurned;
            IntensityLevel = intensityLevel;
            Notes = notes;
            Exercises = exercises;
            IsCompleted = isCompleted;
        }

        public static WorkoutSession Create(
            string userId,
            WorkoutType workoutType,
            DateTime startTime,
            int intensityLevel,
            string? notes = null)
        {
            return new WorkoutSession(
                Guid.NewGuid(),
                userId,
                workoutType,
                startTime,
                null,
                DateTime.UtcNow,
                0,
                0,
                Math.Clamp(intensityLevel, 1, 10),
                notes,
                new List<Exercise>(),
                false
            );
        }

        public WorkoutSession CompleteSession(List<Exercise> exercises)
        {
            var endTime = DateTime.UtcNow;
            var duration = (int)(endTime - StartTime).TotalMinutes;
            var caloriesBurned = exercises.Sum(e => e.CaloriesBurned);

            return new WorkoutSession(
                SessionId,
                UserId,
                WorkoutType,
                StartTime,
                endTime,
                DateTime.UtcNow,
                duration,
                caloriesBurned,
                IntensityLevel,
                Notes,
                exercises,
                true
            );
        }

        public WorkoutSession AddExercise(Exercise exercise)
        {
            var updatedExercises = Exercises.ToList();
            updatedExercises.Add(exercise);

            var updatedCaloriesBurned = updatedExercises.Sum(e => e.CaloriesBurned);

            return new WorkoutSession(
                SessionId,
                UserId,
                WorkoutType,
                StartTime,
                EndTime,
                DateTime.UtcNow,
                DurationMinutes,
                updatedCaloriesBurned,
                IntensityLevel,
                Notes,
                updatedExercises,
                IsCompleted
            );
        }

        public WorkoutSession UpdateCaloriesBurned(int newCaloriesBurned)
        {
            return new WorkoutSession(
                SessionId,
                UserId,
                WorkoutType,
                StartTime,
                EndTime,
                DateTime.UtcNow,
                DurationMinutes,
                newCaloriesBurned,
                IntensityLevel,
                Notes,
                Exercises,
                IsCompleted
            );
        }
    }


    public sealed class Exercise
    {
        public Guid ExerciseId { get; private init; }
        public string Name { get; private init; }
        public int Sets { get; private init; }
        public int Reps { get; private init; }
        public decimal Weight { get; private init; }
        public int DurationMinutes { get; private init; }
        public int CaloriesBurned { get; private init; }
        public string? Notes { get; private init; }

        private Exercise() { }

        private Exercise(Guid exerciseId, string name, int sets, int reps, decimal weight, int durationMinutes, int caloriesBurned, string? notes)
        {
            ExerciseId = exerciseId;
            Name = name;
            Sets = sets;
            Reps = reps;
            Weight = weight;
            DurationMinutes = durationMinutes;
            CaloriesBurned = caloriesBurned;
            Notes = notes;
        }

        public static Exercise Create(
            string name,
            int sets,
            int reps,
            decimal weight,
            int durationMinutes,
            int caloriesBurned,
            string? notes = null)
        {
            return new Exercise(
                Guid.NewGuid(),
                name,
                sets,
                reps,
                weight,
                durationMinutes,
                caloriesBurned,
                notes
            );
        }

        public Exercise Update(
            int sets,
            int reps,
            decimal weight,
            int durationMinutes,
            int caloriesBurned,
            string? notes = null)
        {
            return new Exercise(
                ExerciseId,
                Name,
                sets,
                reps,
                weight,
                durationMinutes,
                caloriesBurned,
                notes ?? Notes
            );
        }
    }
}