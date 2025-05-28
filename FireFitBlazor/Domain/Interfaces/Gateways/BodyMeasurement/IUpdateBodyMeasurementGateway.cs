using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IUpdateBodyMeasurementGateway
{
    Task<bool> UpdateAsync(BodyMeasurement bodyMeasurement);
}
