using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways;
using FireFitBlazor.Infrastructure.Data;

namespace FireFitBlazor.Infrastructure.Gateways
{
    public class BodyMeasurementGateway : BaseGateway<BodyMeasurement>, IBodyMeasurementGateway
    {
        public BodyMeasurementGateway(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> AddAsync(BodyMeasurement measurement)
        {
            return await base.AddAsync(measurement);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<BodyMeasurement> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(m => m.MeasurementId == id);
        }

        public async Task<IEnumerable<BodyMeasurement>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.MeasurementDate)
                .ToListAsync();
        }

        //public Task<bool> UpdateAsync(BodyMeasurement measurement)
        //{
        //    throw new NotImplementedException();
        //}

        Task<BodyMeasurement> IBodyMeasurementGateway.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        //Task<IEnumerable<BodyMeasurement>> IBodyMeasurementGateway.GetByUserIdAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}
    }
} 