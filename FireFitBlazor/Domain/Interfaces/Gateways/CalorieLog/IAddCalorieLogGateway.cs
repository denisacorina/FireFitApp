using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IAddCalorieLogGateway
{
    Task<bool> AddAsync(CalorieLog calorieLog);
}
