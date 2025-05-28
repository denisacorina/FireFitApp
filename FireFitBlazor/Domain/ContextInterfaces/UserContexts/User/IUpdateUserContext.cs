using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IUpdateUserContext
{
    Task<bool> Execute(User user);
}
