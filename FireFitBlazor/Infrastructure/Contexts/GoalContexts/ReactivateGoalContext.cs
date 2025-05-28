using FireFitBlazor.Domain.ContextInterfaces.GoalContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.GoalContexts
{
    public class ReactivateGoalContext : IReactivateGoalContext
    {
        private readonly GoalGateway _goalGateway;

        public ReactivateGoalContext(GoalGateway goalGateway)
        {
            _goalGateway = goalGateway;
        }

        public async Task<Goal> ReactivateGoalAsync(Guid goalId)
        {
            if (goalId == Guid.Empty)
                throw new ArgumentException("GoalId cannot be empty", nameof(goalId));

            var goal = await _goalGateway.GetGoalByIdAsync(goalId);
            if (goal == null)
                throw new InvalidOperationException("Goal not found");

            if (goal.IsActive)
                throw new InvalidOperationException("Goal is already active");

            // Check if user has any other active goals
            var activeGoal = await _goalGateway.GetActiveGoalAsync(goal.UserId);
            if (activeGoal != null)
                throw new InvalidOperationException("User already has an active goal");

            goal.Reactivate();
            return await _goalGateway.UpdateGoalAsync(goal);
        }
    }
} 