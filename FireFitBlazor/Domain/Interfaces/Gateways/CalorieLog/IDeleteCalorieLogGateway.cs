using System.Threading.Tasks;

namespace FireFitBlazor.Domain.Interfaces.Gateways.CalorieLog
{
    public interface IDeleteCalorieLogGateway
    {
        Task<bool> DeleteAsync(int id);
    }
} 