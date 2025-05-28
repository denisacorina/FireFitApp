using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;



public interface IGetFoodLogContext
{
    Task<FoodLog> Execute(int id);
    Task<FoodLog> ExecuteByCalorieLogAndFood(int calorieLogId, int foodId);
}
