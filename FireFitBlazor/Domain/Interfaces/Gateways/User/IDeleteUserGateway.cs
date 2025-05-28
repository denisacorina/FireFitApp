using System.Threading.Tasks;

namespace FireFitBlazor.Domain.Interfaces.Gateways.User
{
    public interface IDeleteUserGateway
    {
        Task<bool> DeleteAsync(int id);
    }
} 