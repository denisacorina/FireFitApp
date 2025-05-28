using FireFitBlazor.Domain.Enums;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Application.DTOs
{
    public class GoalDto
    {
        public Guid GoalId { get; set; }
        public string UserId { get; set; }
        public GoalType Type { get; set; }
        public int CalorieGoal { get; set; }
        public float ProteinGoal { get; set; }
        public float CarbGoal { get; set; }
        public float FatGoal { get; set; }
        public bool IntermittentFasting { get; set; }
        public int FastingWindowHours { get; set; }
        public DietaryPreference DietaryPreference { get; set; }
        public decimal? TargetWeight { get; set; }
        public decimal? TargetBodyFatPercentage { get; set; }
        public DateTime? TargetDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
} 