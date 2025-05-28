using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IBodyMeasurementGateway
{
    Task<BodyMeasurement> GetByIdAsync(Guid id);
    Task<IEnumerable<BodyMeasurement>> GetByUserIdAsync(int userId);
    Task<bool> AddAsync(BodyMeasurement measurement);
    Task<bool> UpdateAsync(BodyMeasurement measurement);
    Task<bool> DeleteAsync(Guid id);
}
