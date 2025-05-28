using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IAddUserProgressGateway
{
    Task<bool> AddAsync(UserProgress userProgress);
}
