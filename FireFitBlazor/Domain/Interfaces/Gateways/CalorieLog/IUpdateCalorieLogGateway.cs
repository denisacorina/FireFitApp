using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IUpdateCalorieLogGateway
{
    Task<bool> UpdateAsync(CalorieLog calorieLog);
}
