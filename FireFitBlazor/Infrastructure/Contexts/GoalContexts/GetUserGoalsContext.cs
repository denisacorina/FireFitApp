using FireFitBlazor.Domain.ContextInterfaces.GoalContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.GoalContexts
{
    public class GetUserGoalsContext : IGetUserGoalsContext
    {
        private readonly GoalGateway _goalGateway;

        public GetUserGoalsContext(GoalGateway goalGateway)
        {
            _goalGateway = goalGateway;
        }

        public async Task<IEnumerable<Goal>> GetUserGoalsAsync(string userId)
        {
            if (userId == String.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            return await _goalGateway.GetUserGoalsAsync(userId);
        }
    }
} 