using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;



public interface IGetListFoodLogContext
{
    Task<IEnumerable<FoodLog>> ExecuteByCalorieLog(int calorieLogId);
    Task<IEnumerable<FoodLog>> ExecuteByFood(int foodId);
}
