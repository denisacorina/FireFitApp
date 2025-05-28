using System.Threading.Tasks;

namespace FireFitBlazor.Domain.ContextInterfaces.FoodContexts.Food
{
    public interface IDeleteFoodContext
    {
        Task<bool> Execute(int id);
    }
} 