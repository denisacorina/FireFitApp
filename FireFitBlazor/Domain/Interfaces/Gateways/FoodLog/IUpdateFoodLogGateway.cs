using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IUpdateFoodLogGateway
{
    Task<bool> UpdateAsync(FoodLog foodLog);
}
