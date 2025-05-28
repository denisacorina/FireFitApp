
using FireFitBlazor.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.ValueObjects
{
    public sealed record WeightGoal
    {
        public decimal TargetWeight { get; set; }
        public FoodTrackingEnums.WeightChangeType ChangeType { get; set; }

        public WeightGoal(decimal targetWeight, FoodTrackingEnums.WeightChangeType changeType)
        {
            if (targetWeight <= 0) throw new ArgumentException("Target weight must be greater than zero.");
            TargetWeight = targetWeight;
            ChangeType = changeType;
        }

        public static WeightGoal Default() => new(0, FoodTrackingEnums.WeightChangeType.Maintain);
    }
}
