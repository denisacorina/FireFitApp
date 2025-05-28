using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IUpdateUserGateway
{
    Task<bool> UpdateAsync(User user);
}
