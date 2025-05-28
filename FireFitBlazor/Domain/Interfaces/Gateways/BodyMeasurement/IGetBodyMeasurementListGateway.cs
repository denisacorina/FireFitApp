using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IGetBodyMeasurementListGateway
{
    Task<IEnumerable<BodyMeasurement>> GetByUserIdAsync(int userId);
    Task<IEnumerable<BodyMeasurement>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
}
