using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.ContextInterfaces.GoalContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.GoalContexts
{
    public class DeleteGoalContext : IDeleteGoalContext
    {
        private readonly GoalGateway _goalGateway;

        public DeleteGoalContext(GoalGateway goalGateway)
        {
            _goalGateway = goalGateway;
        }

        public async Task<bool> DeleteGoalAsync(Guid goalId)
        {
            // Validate goalId
            if (goalId == Guid.Empty)
                throw new ArgumentException("GoalId cannot be empty", nameof(goalId));

            // Check if goal exists
            var existingGoal = await _goalGateway.GetGoalByIdAsync(goalId);
            if (existingGoal == null)
                throw new InvalidOperationException("Goal not found");

            // Delete the goal
            return await _goalGateway.DeleteGoalAsync(goalId);
        }
    }
} 