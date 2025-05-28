using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FireFitBlazor.Infrastructure.Contexts
{
    public class GoalContext : IGoalContext
    {
        private readonly ApplicationDbContext _dbContext;

        public GoalContext(ApplicationDbContext dbContext)
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

        public async Task<Goal> MarkGoalAsCompletedAsync(Guid goalId)
        {
            var goal = await _dbContext.Goals.FindAsync(goalId);
            if (goal == null)
                throw new InvalidOperationException("Goal not found");

            goal.MarkAsCompleted();
            await _dbContext.SaveChangesAsync();
            return goal;
        }

        public async Task<Goal> ReactivateGoalAsync(Guid goalId)
        {
            var goal = await _dbContext.Goals.FindAsync(goalId);
            if (goal == null)
                throw new InvalidOperationException("Goal not found");

            goal.Reactivate();
            await _dbContext.SaveChangesAsync();
            return goal;
        }
    }
} 