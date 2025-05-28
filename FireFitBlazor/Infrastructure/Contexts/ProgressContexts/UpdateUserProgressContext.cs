using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.ProgressContexts
{
    public class UpdateUserProgressContext : IUpdateUserProgressContext
    {
        private readonly ProgressGateway _progressGateway;

        public UpdateUserProgressContext(ProgressGateway progressGateway)
        {
            _progressGateway = progressGateway;
        }

        public async Task<UserProgress> UpdateUserProgressAsync(UserProgress progress)
        {
            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            if (progress.UserId == String.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(progress));

            // Validate weight changes
            if (progress.CurrentWeight <= 0)
                throw new ArgumentException("Current weight must be greater than 0", nameof(progress));

            if (progress.CurrentBodyFatPercentage.HasValue && 
                (progress.CurrentBodyFatPercentage < 0 || progress.CurrentBodyFatPercentage > 100))
                throw new ArgumentException("Body fat percentage must be between 0 and 100", nameof(progress));

            return await _progressGateway.UpdateUserProgressAsync(progress);
        }
    }
} 