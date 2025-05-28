using System.Threading.Tasks;

namespace FireFitBlazor.Domain.ContextInterfaces.FoodContexts.CalorieLog
{
    public interface IDeleteCalorieLogContext
    {
        Task<bool> Execute(int id);
    }
} 