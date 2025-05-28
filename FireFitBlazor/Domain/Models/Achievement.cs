
using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class Achievement
    {
        [Key]
        public Guid AchievementId { get; set; }
        public string UserId { get; set; }
        public AchievementType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EarnedAt { get; set; }

        public static Achievement Create(string userId, AchievementType type, string title, string description)
        {
            return new Achievement
            {
                AchievementId = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                Title = title,
                Description = description,
                EarnedAt = DateTime.UtcNow
            };
        }

        public void Update(AchievementType type, string title, string description)
        {
            Type = type;
            Title = title;
            Description = description;
        }
    }
}
