using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IAddFoodLogGateway
{
    Task<bool> AddAsync(FoodLog foodLog);
}
