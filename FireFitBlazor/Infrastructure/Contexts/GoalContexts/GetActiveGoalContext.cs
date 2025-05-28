using FireFitBlazor.Domain.ContextInterfaces.GoalContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.GoalContexts
{
    public class GetActiveGoalContext : IGetActiveGoalContext
    {
        private readonly GoalGateway _goalGateway;

        public GetActiveGoalContext(GoalGateway goalGateway)
        {
            _goalGateway = goalGateway;
        }

        public async Task<Goal> GetActiveGoalAsync(string userId)
        {
            if (userId == String.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            return await _goalGateway.GetActiveGoalAsync(userId);
        }
    }
} 