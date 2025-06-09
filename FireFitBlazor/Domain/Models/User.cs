using FireFitBlazor.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class User
    {
        [Key]
        public string UserId { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public string PasswordHash { get; init; } = "";
        public int Age { get; init; }
        public Gender Gender { get; init; }

        public int Height { get; init; }
        public decimal StartingWeight { get; init; }
        public decimal TargetWeight { get; init; }
        public WeightGoal WeightGoal { get; init; }
        public ActivityLevel ActivityLevel { get; init; }

        public List<CalorieLog> CalorieLogs { get; init; } = new();
        [PersonalData]
        public List<DietaryPreference> DietaryPreferences { get; init; } = new();

        [PersonalData]
        public string? ProfilePicturePath { get; init; }
        public ExperienceLevel FitnessExperience { get; init; }
        public List<WorkoutType> WorkoutTypes { get; init; } = new();
        public List<WorkoutPreference> WorkoutPreferences { get; init; } = new();

        private User() { }

        [JsonConstructor]
        private User(
    string userId,
    string name,
    string email,
    DateTime createdAt,
    DateTime updatedAt,
    string passwordHash,
    int age,
    Gender gender,
    int height,
    decimal startingWeight,
    decimal targetWeight,
    WeightGoal weightGoal,
    ActivityLevel activityLevel,
    List<CalorieLog> calorieLogs,
    List<DietaryPreference> dietaryPreferences,
    string? profilePicturePath,
    ExperienceLevel fitnessExperience,
    List<WorkoutType> workoutTypes,
    List<WorkoutPreference> workoutPreferences)
        {
            UserId = userId;
            Name = name;
            Email = email;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            PasswordHash = passwordHash;
            Age = age;
            Gender = gender;
            Height = height;
            StartingWeight = startingWeight;
            TargetWeight = targetWeight;
            WeightGoal = weightGoal;
            ActivityLevel = activityLevel;
            CalorieLogs = calorieLogs;
            DietaryPreferences = dietaryPreferences;
            ProfilePicturePath = profilePicturePath;
            FitnessExperience = fitnessExperience;
            WorkoutTypes = workoutTypes;
            WorkoutPreferences = workoutPreferences;
        }

        public static User Create(string userId, string email, string passwordHash, string name, int age, bool isMale, int height, decimal startingWeight, decimal targetWeight, WeightChangeType changeType, ActivityLevel activityLevel, List<DietaryPreference> dietaryPreferences, List<WorkoutType> workoutTypes, string profilePicturePath, ExperienceLevel fitnessExperience)
        {
            return new User(
                   userId,
                   name,
                   email,
                   DateTime.UtcNow,
                   DateTime.UtcNow,
                   passwordHash,
                   age,
                   isMale ? Gender.Male : Gender.Female,
                   height,
                   startingWeight,
                   targetWeight,
                   new WeightGoal(targetWeight, changeType),
                   activityLevel,
                   new List<CalorieLog>(),
                   dietaryPreferences,
                   profilePicturePath,
                   fitnessExperience,
                   workoutTypes,
                   new List<WorkoutPreference>()
            );
        }

        //public User SetupProfile(
        //  int age,
        //  bool isMale,
        //  int height,
        //  decimal targetWeight,
        //  WeightChangeType changeType,
        //  ActivityLevel activityLevel,
        //  List<DietaryPreference> dietaryPreferences,
        //  List<WorkoutType> workoutTypes,
        //  decimal currentWeight)
        //{
        //    if (age is < 18 or > 100)
        //        throw new ArgumentException("Age must be between 18 and 100.");

        //    var gender = isMale ? Gender.Male : Gender.Female;
        //    var weightGoal = new WeightGoal(targetWeight, changeType);

        //    //var preferences = UserPreferences.Create(
        //    //    UserId,
        //    //    dietaryPreferences,
        //    //    (int)GetRecommendedCalories(weightGoal, isMale, activityLevel, age, currentWeight));

        //    //var progress = UserProgress.Create(UserId, currentWeight, currentWeight);

        //    return new User(
        //        UserId,
        //        Name,
        //        Email,
        //        CreatedAt,
        //        DateTime.UtcNow,
        //        PasswordHash,
        //        age,
        //        gender,
        //        height,
        //        weightGoal,
        //        activityLevel,
        //        CalorieLogs ?? new List<CalorieLog>(),
        //        dietaryPreferences,
        //        ProfilePicturePath,
        //        FitnessExperience,
        //        workoutTypes ?? new List<WorkoutType>(),
        //        WorkoutPreferences ?? new List<WorkoutPreference>()
        //    );
        //}


        public User Update(string userId, string email, string name, int age, int height, decimal startingWeight, decimal targetWeight, WeightChangeType changeType, ActivityLevel activityLevel, List<DietaryPreference> dietaryPreferences, List<WorkoutType> workoutTypes, string profilePicturePath, ExperienceLevel fitnessExperience)
        {
            if (age is < 18 or > 100)
                throw new ArgumentException("Age must be between 18 and 100.");

            var weightGoal = new WeightGoal(targetWeight, changeType);

            return new User(
                UserId,
                name,
                email,
                CreatedAt,
                DateTime.UtcNow,
                PasswordHash,
                age,
                Gender,
                height,
                startingWeight,
                targetWeight,
                new WeightGoal(targetWeight, changeType),
                activityLevel,
                new List<CalorieLog>(),
                dietaryPreferences,
                profilePicturePath,
                fitnessExperience,
                workoutTypes,
                new List<WorkoutPreference>()
            );
        }

        public User UpdateFitnessExperience(ExperienceLevel fitnessExperience)
            => With(fitnessExperience: fitnessExperience);

        private User With(
            string userId = null,
            string name = null,
            string email = null,
            DateTime? createdAt = null,
            DateTime? updatedAt = null,
            string passwordHash = null,
            int? age = null,
            Gender? gender = null,
            int? height = null,
            decimal? startingWeight = null,
            decimal? targetWeight = null,
            WeightGoal weightGoal = null,
            ActivityLevel? activityLevel = null,
            List<CalorieLog> calorieLogs = null,
            List<DietaryPreference> dietaryPreferences = null,
            string profilePicturePath = null,
            ExperienceLevel? fitnessExperience = null,
            List<WorkoutType> workoutTypes = null,
            List<WorkoutPreference> workoutPreferences = null)
        {
            return new User(
                userId ?? UserId,
                name ?? Name,
                email ?? Email,
                createdAt ?? CreatedAt,
                updatedAt ?? DateTime.UtcNow,
                passwordHash ?? PasswordHash,
                age ?? Age,
                gender ?? Gender,
                height ?? Height,
                startingWeight ?? StartingWeight,
                targetWeight ?? TargetWeight,
                weightGoal ?? WeightGoal,
                activityLevel ?? ActivityLevel,
                calorieLogs ?? CalorieLogs,
                dietaryPreferences ?? DietaryPreferences,
                profilePicturePath ?? ProfilePicturePath,
                fitnessExperience ?? FitnessExperience,
                workoutTypes ?? WorkoutTypes,
                workoutPreferences ?? WorkoutPreferences
            );
        }

        public User ClearWeightGoal()
        {
            return With(weightGoal: WeightGoal.Default());
        }

        //public void LogDailyCalories(WeightGoal weightGoal, int consumedCalories, bool isMale, int age, ActivityLevel activityLevel)
        //{
        //    int recommendedCalories = (int)GetRecommendedCalories(weightGoal, isMale, activityLevel, age, Progress.StartingWeight);

        //    // Check if there's already a log for today
        //    var existingLog = CalorieLogs.Find(log => log.Date == DateTime.UtcNow.Date);

        //    if (existingLog != null)
        //    {
        //        existingLog.UpdateCalories(consumedCalories);
        //    }
        //    else
        //    {
        //        CalorieLogs.Add(CalorieLog.Create(UserId, consumedCalories, recommendedCalories));
        //    }
        //}

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
        //public decimal CalculateBMI()
        //{
        //    decimal heightInMeters = Height / 100m;
        //    return Progress.StartingWeight / (heightInMeters * heightInMeters);
        //}

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

        //public decimal EstimateWeeksToGoal()
        //{
        //    decimal weightDifference = Math.Abs(Progress.StartingWeight - WeightGoal.TargetWeight);
        //    decimal weeklyChange = WeightGoal.ChangeType switch
        //    {
        //        WeightChangeType.Lose => 0.5m, // Avg safe weight loss: 0.5 kg/week
        //        WeightChangeType.Gain => 0.5m, // Avg safe weight gain: 0.5 kg/week
        //        _ => 0 // No change
        //    };

        //    return weeklyChange == 0 ? 0 : weightDifference / weeklyChange;
        //}

        // 7️⃣ **Track Progress (Weight Lost/Gained)**
        //public decimal GetWeightProgress()
        //{
        //    return Progress.StartingWeight - Progress.CurrentWeight;
        //}
    }
}

