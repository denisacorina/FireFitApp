using FireFitBlazor.Application.DTOs;
using FireFitBlazor.Domain.Enums;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Application.Services
{
    public interface IGoalService
    {
        Task<GoalDto> CreateGoalAsync(string userId, GoalType type, int calorieGoal, float proteinGoal, float carbGoal, float fatGoal,
            bool intermittentFasting, int fastingWindow,
            decimal? targetWeight = null, decimal? targetBodyFatPercentage = null, DateTime? targetDate = null);
        Task<GoalDto> UpdateGoalAsync(GoalDto goal);
        Task<GoalDto> GetActiveGoalAsync(string userId);
        Task<IEnumerable<GoalDto>> GetUserGoalsAsync(string userId);
        Task<bool> DeleteGoalAsync(Guid goalId);
        Task<GoalDto> MarkGoalAsCompletedAsync(Guid goalId);
        Task<GoalDto> ReactivateGoalAsync(Guid goalId);
    }
} 