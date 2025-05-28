using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.ProgressContexts
{
    public class GetBodyMeasurementsContext : IGetBodyMeasurementsContext
    {
        private readonly ProgressGateway _progressGateway;

        public GetBodyMeasurementsContext(ProgressGateway progressGateway)
        {
            _progressGateway = progressGateway;
        }

        public async Task<IEnumerable<BodyMeasurement>> GetBodyMeasurementsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (userId == String.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            // Validate date range
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new ArgumentException("Start date cannot be after end date");

            return await _progressGateway.GetBodyMeasurementsAsync(userId, startDate, endDate);
        }
    }
} 