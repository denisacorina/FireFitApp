using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using FireFit.Shared.Enums;
using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Adapters;

namespace FireFitBlazor.Infrastructure.Adapters;

public class GoalServiceAdapter : IGoalService
{
    private readonly IGoalContext _goals;
    public GoalServiceAdapter(IGoalContext goals) { _goals = goals; }

    public async Task<GoalDto?> GetActiveGoalAsync(string userId, CancellationToken ct = default)
    {
        var g = await _goals.GetActiveGoalAsync(userId);
        return g != null ? Map(g) : null;
    }

    public async Task CreateGoalAsync(GoalDto goal, CancellationToken ct = default)
    {
        var created = await _goals.CreateGoalAsync(Goal.Create(
            goal.UserId,
            goal.Type.ToDomain(),
            goal.CalorieGoal,
            goal.ProteinGoal,
            goal.CarbGoal,
            goal.FatGoal,
            goal.IntermittentFasting,
            goal.FastingWindowHours,
            goal.TargetWeight,
            null,
            goal.TargetDateUtc));
    }

    public async Task UpdateGoalAsync(GoalDto goal, CancellationToken ct = default)
    {
        var existing = await _goals.GetActiveGoalAsync(goal.UserId);
        if (existing == null) return;
        existing.Update(
            goal.Type.ToDomain(),
            goal.CalorieGoal,
            goal.ProteinGoal,
            goal.CarbGoal,
            goal.FatGoal,
            goal.IntermittentFasting,
            goal.FastingWindowHours,
            goal.TargetWeight,
            null,
            goal.TargetDateUtc);
        await _goals.UpdateGoalAsync(existing);
    }

    public Task MarkGoalCompletedAsync(string goalId, CancellationToken ct = default)
        => Guid.TryParse(goalId, out var gid)
            ? _goals.MarkGoalAsCompletedAsync(gid)
            : Task.CompletedTask;

    public async Task<IEnumerable<GoalDto>> GetUserGoalsAsync(string userId, CancellationToken ct = default)
    {
        var list = await _goals.GetUserGoalsAsync(userId);
        return list.Select(Map).ToList();
    }

    public Task DeleteGoalAsync(string goalId, CancellationToken ct = default)
        => Guid.TryParse(goalId, out var gid)
            ? _goals.DeleteGoalAsync(gid)
                .ContinueWith(_ => { }, ct)
            : Task.CompletedTask;

    public Task ReactivateGoalAsync(string goalId, CancellationToken ct = default)
        => Guid.TryParse(goalId, out var gid)
            ? _goals.ReactivateGoalAsync(gid)
                .ContinueWith(_ => { }, ct)
            : Task.CompletedTask;

    private static GoalDto Map(Goal goal) => new GoalDto
    {
        Id = goal.GoalId.ToString(),
        UserId = goal.UserId,
        Type = goal.Type.ToShared(),
        CalorieGoal = (int)goal.NutritionalGoal.Calories,
        ProteinGoal = (int)goal.NutritionalGoal.Proteins,
        CarbGoal = (int)goal.NutritionalGoal.Carbs,
        FatGoal = (int)goal.NutritionalGoal.Fats,
        IntermittentFasting = goal.IntermittentFasting,
        FastingWindowHours = goal.FastingWindowHours,
        TargetWeight = goal.TargetWeight,
        TargetDateUtc = goal.TargetDate,
    };
}
