using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IAddUserGateway
{
    Task<bool> AddAsync(User user);
}
