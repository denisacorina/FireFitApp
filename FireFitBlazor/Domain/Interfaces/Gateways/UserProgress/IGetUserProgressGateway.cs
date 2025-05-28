using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IGetUserProgressGateway
{
    Task<UserProgress> GetByIdAsync(string id);
    Task<UserProgress> GetByUserIdAndDateAsync(string userId, DateTime date);
}
