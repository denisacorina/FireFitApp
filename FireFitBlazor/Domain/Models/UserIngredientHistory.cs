using System;
using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.Models
{
    public sealed class UserIngredientHistory
    {
        [Key]
        public Guid HistoryId { get; init; }
        public string UserId { get; init; }
        public Guid IngredientId { get; init; }
        public string IngredientName { get; init; }
        public DateTime LastUsed { get; init; }
        public int UseCount { get; init; }

        private UserIngredientHistory() { }

        private UserIngredientHistory(
            Guid historyId,
            string userId,
            Guid ingredientId,
            string ingredientName,
            DateTime lastUsed,
            int useCount)
        {
            HistoryId = historyId;
            UserId = userId;
            IngredientId = ingredientId;
            IngredientName = ingredientName;
            LastUsed = lastUsed;
            UseCount = useCount;
        }

        public static UserIngredientHistory Create(
            string userId,
            Guid ingredientId,
            string ingredientName)
        {
            return new UserIngredientHistory(
                historyId: Guid.NewGuid(),
                userId: userId,
                ingredientId: ingredientId,
                ingredientName: ingredientName,
                lastUsed: DateTime.UtcNow,
                useCount: 1
            );
        }

        public UserIngredientHistory UpdateUsage()
        {
            return With(
                lastUsed: DateTime.UtcNow,
                useCount: UseCount + 1
            );
        }

        private UserIngredientHistory With(
            Guid? historyId = null,
            string userId = null,
            Guid? ingredientId = null,
            string ingredientName = null,
            DateTime? lastUsed = null,
            int? useCount = null)
        {
            return new UserIngredientHistory(
                historyId ?? HistoryId,
                userId ?? UserId,
                ingredientId ?? IngredientId,
                ingredientName ?? IngredientName,
                lastUsed ?? LastUsed,
                useCount ?? UseCount
            );
        }
    }
} 