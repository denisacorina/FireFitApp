using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.ContextInterfaces
{
    public interface IGoalContext
    {
        Task<Goal> CreateGoalAsync(Goal goal);
        Task<Goal> UpdateGoalAsync(Goal goal);
        Task<Goal> GetActiveGoalAsync(string userId);
        Task<IEnumerable<Goal>> GetUserGoalsAsync(string userId);
        Task<bool> DeleteGoalAsync(Guid goalId);
        Task<Goal> MarkGoalAsCompletedAsync(Guid goalId);
        Task<Goal> ReactivateGoalAsync(Guid goalId);
    }
} 