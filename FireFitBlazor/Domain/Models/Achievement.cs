using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public sealed class Achievement
    {
        [Key]
        public Guid AchievementId { get; private init; }
        public string UserId { get; private init; }
        public AchievementType Type { get; private init; }
        public string Title { get; private init; }
        public string Description { get; private init; }
        public DateTime EarnedAt { get; private init; }

        private Achievement() { }

        private Achievement(Guid achievementId, string userId, AchievementType type, string title, string description, DateTime earnedAt)
        {
            AchievementId = achievementId;
            UserId = userId;
            Type = type;
            Title = title;
            Description = description;
            EarnedAt = earnedAt;
        }

        public static Achievement Create(string userId, AchievementType type, string title, string description)
        {
            return new Achievement(
                Guid.NewGuid(),
                userId,
                type,
                title,
                description,
                DateTime.UtcNow
            );
        }

        public Achievement Update(AchievementType type, string title, string description)
        {
            return new Achievement(
                AchievementId,
                UserId,
                type,
                title,
                description,
                EarnedAt
            );
        }
    }
}
