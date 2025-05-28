using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireFitBlazor.Domain.Models
{
    public sealed class CalorieLog
    {
        [Key]
        public Guid LogId { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; } 
        public int ConsumedCalories { get; set; }
        public int RecommendedCalories { get; set; }

        private CalorieLog() { }

        public static CalorieLog Create(string userId, int consumedCalories, int recommendedCalories)
        {
            return new CalorieLog
            {
                LogId = Guid.NewGuid(),
                UserId = userId,
                Date = DateTime.UtcNow.Date, 
                ConsumedCalories = consumedCalories,
                RecommendedCalories = recommendedCalories
            };
        }

        public void UpdateCalories(int consumedCalories)
        {
            ConsumedCalories = consumedCalories;
        }

        public int GetCalorieDifference()
        {
            return ConsumedCalories - RecommendedCalories;
        }
    }
}
