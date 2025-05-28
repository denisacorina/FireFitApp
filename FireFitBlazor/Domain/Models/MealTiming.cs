
using FireFitBlazor.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.Models
{
    public sealed class MealTiming
    {
        [Key]
        public Guid MealTimingId { get; set; }
        public string UserId { get; set; }
        public DateTime MealStartTime { get; set; }
        public DateTime MealEndTime { get; set; }
        public FoodTrackingEnums.MealType MealType { get; set; }
        public bool IsFastingPeriod { get; set; }

        private MealTiming() { }

        public static MealTiming Create(string userId, DateTime start, DateTime end, FoodTrackingEnums.MealType mealType, bool fasting)
        {
            return new MealTiming
            {
                MealTimingId = Guid.NewGuid(),
                UserId = userId,
                MealStartTime = start,
                MealEndTime = end,
                MealType = mealType,
                IsFastingPeriod = fasting
            };
        }

        public void Update(DateTime start, DateTime end, FoodTrackingEnums.MealType mealType, bool fasting)
        {
            MealStartTime = start;
            MealEndTime = end;
            MealType = mealType;
            IsFastingPeriod = fasting;
        }

        public void Clear()
        {
            MealStartTime = DateTime.MinValue;
            MealEndTime = DateTime.MinValue;
            IsFastingPeriod = false;
        }
    }
}
