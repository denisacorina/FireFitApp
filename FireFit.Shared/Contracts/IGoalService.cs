using FireFit.Shared.DTOs;

namespace FireFit.Shared.Contracts;

public interface IGoalService
{
    Task<GoalDto?> GetActiveGoalAsync(string userId, CancellationToken ct = default);
    Task CreateGoalAsync(GoalDto goal, CancellationToken ct = default);
    Task UpdateGoalAsync(GoalDto goal, CancellationToken ct = default);
    Task MarkGoalCompletedAsync(string goalId, CancellationToken ct = default);
    Task<IEnumerable<GoalDto>> GetUserGoalsAsync(string userId, CancellationToken ct = default);
    Task DeleteGoalAsync(string goalId, CancellationToken ct = default);
    Task ReactivateGoalAsync(string goalId, CancellationToken ct = default);
}
