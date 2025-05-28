using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.ProgressContexts
{
    public class DeleteBodyMeasurementContext : IDeleteBodyMeasurementContext
    {
        private readonly ProgressGateway _progressGateway;

        public DeleteBodyMeasurementContext(ProgressGateway progressGateway)
        {
            _progressGateway = progressGateway;
        }

        public async Task<bool> DeleteBodyMeasurementAsync(Guid measurementId)
        {
            if (measurementId == Guid.Empty)
                throw new ArgumentException("MeasurementId cannot be empty", nameof(measurementId));

            var measurement = await _progressGateway.DeleteBodyMeasurementAsync(measurementId);

            return true;
        }
    }
}

 