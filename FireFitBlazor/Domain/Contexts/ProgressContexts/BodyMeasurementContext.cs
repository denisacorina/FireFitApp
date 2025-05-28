using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Interfaces.Gateways;

namespace FireFitBlazor.Domain.Contexts.ProgressContexts
{
    public class BodyMeasurementContext : IBodyMeasurementContext
    {
        private readonly IBodyMeasurementGateway _bodyMeasurementGateway;

        public BodyMeasurementContext(IBodyMeasurementGateway bodyMeasurementGateway)
        {
            _bodyMeasurementGateway = bodyMeasurementGateway;
        }

        public async Task<BodyMeasurement> Execute(Guid id)
        {


            return await _bodyMeasurementGateway.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BodyMeasurement>> GetUserMeasurements(int userId)
        {
            if (userId <= 0)
                return new List<BodyMeasurement>();

            return await _bodyMeasurementGateway.GetByUserIdAsync(userId);
        }

        public async Task<bool> AddMeasurement(BodyMeasurement measurement)
        {
            if (measurement == null)
                return false;

            return await _bodyMeasurementGateway.AddAsync(measurement);
        }

        public async Task<bool> UpdateMeasurement(BodyMeasurement measurement)
        {
            if (measurement == null)
                return false;

            return await _bodyMeasurementGateway.UpdateAsync(measurement);
        }

        public async Task<bool> DeleteMeasurement(Guid id)
        {
  
            return await _bodyMeasurementGateway.DeleteAsync(id);
        }

        public Task<IEnumerable<BodyMeasurement>> GetUserMeasurements(string userId)
        {
            throw new NotImplementedException();
        }
    }
} 