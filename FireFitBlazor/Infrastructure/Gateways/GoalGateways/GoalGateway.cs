using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public interface IGoalGateway
{
    Task<Goal> CreateGoalAsync(Goal goal);
    Task<Goal> UpdateGoalAsync(Goal goal);
    Task<Goal> GetGoalByIdAsync(Guid goalId);
    Task<Goal> GetActiveGoalAsync(string userId);
    Task<IEnumerable<Goal>> GetUserGoalsAsync(string userId);
    Task<bool> DeleteGoalAsync(Guid goalId);
}
public class GoalGateway : IGoalGateway
{
    protected readonly ApplicationDbContext _dbContext;

    public GoalGateway(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Goal> CreateGoalAsync(Goal goal)
    {
        _dbContext.Goals.Add(goal);
        await _dbContext.SaveChangesAsync();
        return goal;
    }

    public async Task<Goal> UpdateGoalAsync(Goal goal)
    {
        _dbContext.Goals.Update(goal);
        await _dbContext.SaveChangesAsync();
        return goal;
    }

    public async Task<Goal> GetGoalByIdAsync(Guid goalId)
    {
        return await _dbContext.Goals.FindAsync(goalId);
    }

    public async Task<Goal> GetActiveGoalAsync(string userId)
    {
        return await _dbContext.Goals
            .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
    }

    public async Task<IEnumerable<Goal>> GetUserGoalsAsync(string userId)
    {
        return await _dbContext.Goals
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> DeleteGoalAsync(Guid goalId)
    {
        var goal = await _dbContext.Goals.FindAsync(goalId);
        if (goal == null)
            return false;

        _dbContext.Goals.Remove(goal);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
