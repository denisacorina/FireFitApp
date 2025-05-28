using FireFitBlazor.Domain.Enums;
using FireFitBlazor.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;
using static FireFitBlazor.Domain.Enums.GoalType;

namespace FireFitBlazor.Domain.Models
{
    public sealed class Goal
    {
        [Key]
        public Guid GoalId { get; set; }
        public string UserId { get; set; }
        public GoalType Type { get; set; }
        public NutritionalInfo NutritionalGoal { get; set; }
        public bool IntermittentFasting { get; set; }
        public int FastingWindowHours { get; set; }
        public DietaryPreference DietaryPreference { get; set; }
        public decimal? TargetWeight { get; set; }
        public decimal? TargetBodyFatPercentage { get; set; }
        public DateTime? TargetDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

 

        public static Goal Create(
            string userId,
            GoalType type,
            int calorieGoal,
            float proteinGoal,
            float carbGoal,
            float fatGoal,
            bool intermittentFasting,
            int fastingWindow,
            DietaryPreference dietaryPreference,
            decimal? targetWeight = null,
            decimal? targetBodyFatPercentage = null,
            DateTime? targetDate = null)
        {
            return new Goal
            {
                GoalId = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                NutritionalGoal = NutritionalInfo.Create(calorieGoal, proteinGoal, carbGoal, fatGoal),
                IntermittentFasting = intermittentFasting,
                FastingWindowHours = fastingWindow,
                DietaryPreference = dietaryPreference,
                TargetWeight = targetWeight,
                TargetBodyFatPercentage = targetBodyFatPercentage,
                TargetDate = targetDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(
            GoalType type,
            int calorieGoal,
            float proteinGoal,
            float carbGoal,
            float fatGoal,
            bool intermittentFasting,
            int fastingWindow,
            DietaryPreference dietaryPreference,
            decimal? targetWeight = null,
            decimal? targetBodyFatPercentage = null,
            DateTime? targetDate = null)
        {
            Type = type;
            NutritionalGoal = NutritionalInfo.Create(calorieGoal, proteinGoal, carbGoal, fatGoal);
            IntermittentFasting = intermittentFasting;
            FastingWindowHours = fastingWindow;
            DietaryPreference = dietaryPreference;
            TargetWeight = targetWeight;
            TargetBodyFatPercentage = targetBodyFatPercentage;
            TargetDate = targetDate;
        }

        public void MarkAsCompleted()
        {
            IsActive = false;
            CompletedAt = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            IsActive = true;
            CompletedAt = null;
        }

        public void Clear()
        {
            NutritionalGoal = NutritionalInfo.Zero();
            DietaryPreference = DietaryPreference.None;
            TargetWeight = null;
            TargetBodyFatPercentage = null;
            TargetDate = null;
            IsActive = false;
            CompletedAt = DateTime.UtcNow;
        }
    }
}
