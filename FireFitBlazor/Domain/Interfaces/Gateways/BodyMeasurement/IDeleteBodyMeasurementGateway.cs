using System.Threading.Tasks;

namespace FireFitBlazor.Domain.Interfaces.Gateways.BodyMeasurement
{
    public interface IDeleteBodyMeasurementGateway
    {
        Task<bool> DeleteAsync(int id);
    }
} 