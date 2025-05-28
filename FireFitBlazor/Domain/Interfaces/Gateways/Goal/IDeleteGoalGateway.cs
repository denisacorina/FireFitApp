using System.Threading.Tasks;

namespace FireFitBlazor.Domain.Interfaces.Gateways.Goal
{
    public interface IDeleteGoalGateway
    {
        Task<bool> DeleteAsync(Guid id);
    }
} 