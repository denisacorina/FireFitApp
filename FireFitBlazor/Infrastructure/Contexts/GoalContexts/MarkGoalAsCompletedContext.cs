using FireFitBlazor.Domain.ContextInterfaces.GoalContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.GoalContexts
{
    public class MarkGoalAsCompletedContext : IMarkGoalAsCompletedContext
    {
        private readonly GoalGateway _goalGateway;

        public MarkGoalAsCompletedContext(GoalGateway goalGateway)
        {
            _goalGateway = goalGateway;
        }

        public async Task<Goal> MarkGoalAsCompletedAsync(Guid goalId)
        {
            if (goalId == Guid.Empty)
                throw new ArgumentException("GoalId cannot be empty", nameof(goalId));

            var goal = await _goalGateway.GetGoalByIdAsync(goalId);
            if (goal == null)
                throw new InvalidOperationException("Goal not found");

            if (!goal.IsActive)
                throw new InvalidOperationException("Goal is not active");

            goal.MarkAsCompleted();
            return await _goalGateway.UpdateGoalAsync(goal);
        }
    }
} 