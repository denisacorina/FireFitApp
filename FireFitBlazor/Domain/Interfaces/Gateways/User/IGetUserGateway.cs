using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IGetUserGateway
{
    Task<User> GetByIdAsync(string id);
    Task<User> GetByEmailAsync(string email);
}
