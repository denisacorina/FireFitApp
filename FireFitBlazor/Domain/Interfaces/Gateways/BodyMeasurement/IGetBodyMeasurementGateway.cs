using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IGetBodyMeasurementGateway
{
    Task<BodyMeasurement> GetByIdAsync(int id);
    Task<BodyMeasurement> GetByUserIdAndDateAsync(int userId, DateTime date);
}
