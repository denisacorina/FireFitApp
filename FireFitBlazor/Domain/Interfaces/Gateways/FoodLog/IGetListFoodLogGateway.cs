using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IGetListFoodLogGateway
{
    Task<IEnumerable<FoodLog>> GetByCalorieLogIdAsync(int calorieLogId);
    Task<IEnumerable<FoodLog>> GetByFoodIdAsync(int foodId);
}
