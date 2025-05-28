using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.ContextInterfaces.GoalContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.GoalContexts
{
    public class CreateGoalContext : ICreateGoalContext
    {
        private readonly GoalGateway _goalGateway;

        public CreateGoalContext(GoalGateway goalGateway)
        {
            _goalGateway = goalGateway;
        }

        public async Task<Goal> CreateGoalAsync(Goal goal)
        {
            // Validate goal data
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            if (goal.UserId == String.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(goal));

            // Check if user already has an active goal
            var activeGoal = await _goalGateway.GetActiveGoalAsync(goal.UserId);
            if (activeGoal != null)
                throw new InvalidOperationException("User already has an active goal");

            // Create the goal
            return await _goalGateway.CreateGoalAsync(goal);
        }
    }
} 