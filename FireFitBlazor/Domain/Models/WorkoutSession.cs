using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class WorkoutSession
    {
        [Key]
        public Guid SessionId { get; set; }
        public string UserId { get; set; }
        public WorkoutType WorkoutType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
        public int IntensityLevel { get; set; } // 1-10
        public string? Notes { get; set; }
        public List<Exercise> Exercises { get; set; } = new();
        public bool IsCompleted { get; set; }

        private WorkoutSession() { }

        public static WorkoutSession Create(
            string userId,
            WorkoutType workoutType,
            DateTime startTime,
            int intensityLevel,
            string? notes = null)
        {
            return new WorkoutSession
            {
                SessionId = Guid.NewGuid(),
                UserId = userId,
                WorkoutType = workoutType,
                StartTime = startTime,
                IntensityLevel = Math.Clamp(intensityLevel, 1, 10),
                Notes = notes,
                IsCompleted = false
            };
        }

        public void CompleteSession(
            DateTime endTime,
            int caloriesBurned,
            List<Exercise> exercises)
        {
            EndTime = endTime;
            DurationMinutes = (int)(EndTime - StartTime).TotalMinutes;
            CaloriesBurned = caloriesBurned;
            Exercises = exercises;
            IsCompleted = true;
        }

        public void UpdateCaloriesBurned(int newCaloriesBurned)
        {
            CaloriesBurned = newCaloriesBurned;
        }

        public void AddExercise(Exercise exercise)
        {
            Exercises.Add(exercise);
            // Recalculate total calories burned
            CaloriesBurned = Exercises.Sum(e => e.CaloriesBurned);
        }
    }

    public class Exercise
    {
        public Guid ExerciseId { get; set; }
        public string Name { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal Weight { get; set; }
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
        public string? Notes { get; set; }

        private Exercise() { }

        public static Exercise Create(
            string name,
            int sets,
            int reps,
            decimal weight,
            int durationMinutes,
            int caloriesBurned,
            string? notes = null)
        {
            return new Exercise
            {
                ExerciseId = Guid.NewGuid(),
                Name = name,
                Sets = sets,
                Reps = reps,
                Weight = weight,
                DurationMinutes = durationMinutes,
                CaloriesBurned = caloriesBurned,
                Notes = notes
            };
        }

        public void Update(
            int sets,
            int reps,
            decimal weight,
            int durationMinutes,
            int caloriesBurned,
            string? notes = null)
        {
            Sets = sets;
            Reps = reps;
            Weight = weight;
            DurationMinutes = durationMinutes;
            CaloriesBurned = caloriesBurned;
            Notes = notes;
        }
    }
}