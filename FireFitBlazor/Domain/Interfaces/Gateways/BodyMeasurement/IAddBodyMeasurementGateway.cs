using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IAddBodyMeasurementGateway
{
    Task<bool> AddAsync(BodyMeasurement bodyMeasurement);
}
