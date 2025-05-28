using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.ContextInterfaces.GoalContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.GoalContexts
{
    public class UpdateGoalContext : IUpdateGoalContext
    {
        private readonly GoalGateway _goalGateway;

        public UpdateGoalContext(GoalGateway goalGateway)
        {
            _goalGateway = goalGateway;
        }

        public async Task<Goal> Execute(Goal goal)
        {
            // Validate goal data
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            if (goal.GoalId == Guid.Empty)
                throw new ArgumentException("GoalId cannot be empty", nameof(goal));

            // Check if goal exists
            var existingGoal = await _goalGateway.GetGoalByIdAsync(goal.GoalId);
            if (existingGoal == null)
                throw new InvalidOperationException("Goal not found");

            // Update the goal
            return await _goalGateway.UpdateGoalAsync(goal);
        }

        Task<bool> IUpdateGoalContext.Execute(Goal goal)
        {
            throw new NotImplementedException();
        }
    }
} 