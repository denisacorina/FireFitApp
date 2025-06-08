using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IUpdateUserGateway
{
    Task<User> GetUserById(string userId);
    Task UpdateAsync(User user);
}
