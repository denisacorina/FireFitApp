using System.Threading.Tasks;

namespace FireFitBlazor.Domain.Interfaces.Gateways.UserProgress
{
    public interface IDeleteUserProgressGateway
    {
        Task<bool> DeleteAsync(string id);
    }
} 