using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IGetCalorieLogGateway
{
    Task<CalorieLog> GetByIdAsync(int id);
    Task<CalorieLog> GetByUserIdAndDateAsync(int userId, DateTime date);
}
