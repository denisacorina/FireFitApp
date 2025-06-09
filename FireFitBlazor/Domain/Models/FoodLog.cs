using FireFitBlazor.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class FoodLog
    {
        [Key]
        public Guid FoodLogId { get; set; }
        public string UserId { get; set; }
        public string FoodName { get; set; }
        public NutritionalInfo NutritionalInfo { get; set; }
        public DateTime Timestamp { get; set; }
        public MealType? MealType { get; set; }

        public int PortionSize = 1;

        public string Unit = "serving";
        public FoodLog() { }
        public FoodLog(string name, float calories, float proteins, float carbs, float fats)
        {
            FoodName = name;
            NutritionalInfo = new NutritionalInfo(calories, proteins, carbs, fats);
            Timestamp = DateTime.UtcNow;
        }

        public static FoodLog Create(string userId, string foodName, int calories, float proteins, float carbs, float fats)
        {
            return new FoodLog
            {
                FoodLogId = Guid.NewGuid(),
                UserId = userId,
                FoodName = foodName,
                NutritionalInfo = NutritionalInfo.Create(calories, proteins, carbs, fats),
                Timestamp = DateTime.UtcNow
            };
        }

        public void Update(string foodName, int calories, float proteins, float carbs, float fats)
        {
            FoodName = FoodName;
            NutritionalInfo = NutritionalInfo.Create(calories, proteins, carbs, fats);
        }

        public void Clear()
        {
            FoodName = string.Empty;
            NutritionalInfo = NutritionalInfo.Zero();
        }
    }
}
