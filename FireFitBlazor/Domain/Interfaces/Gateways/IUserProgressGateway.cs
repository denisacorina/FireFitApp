using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IUserProgressGateway
{
    Task<UserProgress> GetByUserIdAsync(string userId);
    Task<bool> AddAsync(UserProgress progress);
    Task<bool> UpdateAsync(UserProgress progress);
}
