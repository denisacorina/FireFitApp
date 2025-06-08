using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.ProgressContexts
{
    public class GetUserProgressContext : IGetUserProgressContext
    {
        private readonly IProgressGateway _progressGateway;

        public GetUserProgressContext(IProgressGateway progressGateway)
        {
            _progressGateway = progressGateway;
        }

        public async Task<UserProgress> GetUserProgressAsync(string userId)
        {
            if (userId == string.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            var userProgress = await _progressGateway.GetUserProgressAsync(userId);

            if (userProgress == null)
                throw new KeyNotFoundException($"No progress found for user with ID {userId}");

            return userProgress;
        }
    }
} 