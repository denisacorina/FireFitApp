using System.Threading.Tasks;

namespace FireFitBlazor.Domain.ContextInterfaces.FoodContexts.FoodLog
{
    public interface IDeleteFoodLogContext
    {
        Task<bool> Execute(Guid id);
    }
} 