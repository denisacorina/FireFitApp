using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.ProgressContexts
{
    public class GetLatestBodyMeasurementContext : IGetLatestBodyMeasurementContext
    {
        private readonly IProgressGateway _progressGateway;

        public GetLatestBodyMeasurementContext(IProgressGateway progressGateway)
        {
            _progressGateway = progressGateway;
        }

        public async Task<BodyMeasurement> GetLatestBodyMeasurementAsync(string userId)
        {
            if (userId == String.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            return await _progressGateway.GetLatestBodyMeasurementAsync(userId);
        }
    }
} 