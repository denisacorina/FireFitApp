using FireFitBlazor.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class User
    {
        [Key]
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string PasswordHash { get; set; } = "";
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public decimal CurrentWeight;
        public decimal StartingWeight;
        public UserProgress Progress { get;  set; }
        public UserPreferences Preferences { get;  set; }

        public int Height { get; set; }
        public WeightGoal WeightGoal { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public List<CalorieLog> CalorieLogs { get; set; } = new List<CalorieLog>();

        [PersonalData]
        public List<DietaryPreference> DietaryPreferences { get; set; } = new();
        [PersonalData]
        public string? ProfilePicturePath { get; set; }
        public ExperienceLevel FitnessExperience { get; set; }
        public List<WorkoutPreference> WorkoutPreferences { get; set; } = new();

        public static User Create(string userId, string email, string name)
        {
            return new User
            {
                UserId = userId,
                Email = email,
                Name = name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
        public void SetupProfile(int age, bool isMale, decimal currentWeight, int height, decimal targetWeight, WeightChangeType changeType, ActivityLevel activityLevel, List<DietaryPreference> dietaryPreferences)
        {
            if (age is < 18 or > 100)
                throw new ArgumentException("Age must be between 18 and 100.");

            Age = age;
            Gender = isMale ? Gender.Male : Gender.Female;
            Height = height;
            CurrentWeight = currentWeight;
            StartingWeight = currentWeight;
            WeightGoal = new WeightGoal(targetWeight, changeType);
            ActivityLevel = activityLevel;
            Progress = UserProgress.Create(UserId, currentWeight, currentWeight);
            UpdatedAt = DateTime.UtcNow;
        }


        public void Update(string name, string email, int age, bool isMale, decimal currentWeight, int height, decimal targetWeight, WeightChangeType changeType, ActivityLevel activityLevel, List<DietaryPreference> dietaryPreferences)
        {
            if (age is < 18 or > 100) throw new ArgumentException("Age must be between 18 and 100.");

            var gender = isMale ? Gender.Male : Gender.Female;

            Name = name;
            Email = email;
            Age = age;
            Gender = isMale ? Gender.Male : Gender.Female;
            Height = height;
            WeightGoal = new WeightGoal(targetWeight, changeType);
            ActivityLevel = activityLevel;
          
            UpdatedAt = DateTime.UtcNow;

            Progress = UserProgress.Create(UserId, StartingWeight, CurrentWeight);
            var weightGoal = new WeightGoal(targetWeight, changeType);

            var newCalorieGoal = GetRecommendedCalories(weightGoal, isMale, activityLevel, age, Progress.StartingWeight);
            Preferences = Preferences.Update(dietaryPreferences, (int)newCalorieGoal);
            CalorieLogs = new List<CalorieLog>();
        }

        public void ClearWeightGoal()
        {
            WeightGoal = WeightGoal.Default();
        }

        public void LogDailyCalories(WeightGoal weightGoal, int consumedCalories, bool isMale, int age, ActivityLevel activityLevel)
        {
            int recommendedCalories = (int)GetRecommendedCalories(weightGoal, isMale, activityLevel, age, Progress.StartingWeight);

            // Check if there's already a log for today
            var existingLog = CalorieLogs.Find(log => log.Date == DateTime.UtcNow.Date);

            if (existingLog != null)
            {
                existingLog.UpdateCalories(consumedCalories);
            }
            else
            {
                CalorieLogs.Add(CalorieLog.Create(UserId, consumedCalories, recommendedCalories));
            }
        }

        public int GetCaloriesToday()
        {
            var todayLog = CalorieLogs.Find(log => log.Date == DateTime.UtcNow.Date);
            return todayLog?.ConsumedCalories ?? 0;
        }

        public int GetCalorieDifferenceToday()
        {
            var todayLog = CalorieLogs.Find(log => log.Date == DateTime.UtcNow.Date);
            return todayLog?.GetCalorieDifference() ?? 0;
        }

        public static decimal CalculateBMR(bool isMale, int age, decimal startingWeight)
        {
            // Mifflin-St Jeor Equation
            return isMale
                ? (10 * startingWeight) + (6.25m * startingWeight) - (5 * age) + 5
                : (10 * startingWeight) + (6.25m * startingWeight) - (5 * age) - 161;
        }

        public static decimal CalculateTDEE(bool isMale, ActivityLevel activityLevel, int age, decimal startingWeight)
        {
            decimal bmr = CalculateBMR(isMale, age, startingWeight);
            return bmr * ActivityMultiplier(activityLevel);
        }

        private static decimal ActivityMultiplier(ActivityLevel level)
        {
            return level switch
            {
                ActivityLevel.Sedentary => 1.2m,
                ActivityLevel.Light => 1.375m,
                ActivityLevel.Moderate => 1.55m,
                ActivityLevel.Active => 1.725m,
                ActivityLevel.VeryActive => 1.9m,
                _ => 1.2m
            };
        }

        // **Calculate BMI** (Body Mass Index)
        public decimal CalculateBMI()
        {
            decimal heightInMeters = Height / 100m;
            return Progress.StartingWeight / (heightInMeters * heightInMeters);
        }

        public static decimal GetRecommendedCalories(WeightGoal weightGoal, bool isMale, ActivityLevel activityLevel, int age, decimal startingWeight)
        {
            var tdee = CalculateTDEE(isMale, activityLevel, age, startingWeight);
            return weightGoal.ChangeType switch
            {
                WeightChangeType.Lose => tdee - 500, // Reduce by 500 kcal for weight loss
                WeightChangeType.Gain => tdee + 500, // Increase by 500 kcal for weight gain
                _ => tdee // Maintain weight
            };
        }

        public decimal GetMinimumCalories(bool isMale, int age, decimal progress)
        {
            return CalculateBMR(isMale, age, progress) * 1.1m; // 10% above BMR to prevent starvation mode
        }

        public decimal EstimateWeeksToGoal()
        {
            decimal weightDifference = Math.Abs(Progress.StartingWeight - WeightGoal.TargetWeight);
            decimal weeklyChange = WeightGoal.ChangeType switch
            {
                WeightChangeType.Lose => 0.5m, // Avg safe weight loss: 0.5 kg/week
                WeightChangeType.Gain => 0.5m, // Avg safe weight gain: 0.5 kg/week
                _ => 0 // No change
            };

            return weeklyChange == 0 ? 0 : weightDifference / weeklyChange;
        }

        // 7️⃣ **Track Progress (Weight Lost/Gained)**
        public decimal GetWeightProgress()
        {
            return Progress.StartingWeight - Progress.CurrentWeight;
        }
    }
}

