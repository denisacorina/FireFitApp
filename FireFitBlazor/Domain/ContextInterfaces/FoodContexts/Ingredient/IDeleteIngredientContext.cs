using System.Threading.Tasks;

namespace FireFitBlazor.Domain.ContextInterfaces.FoodContexts.Ingredient
{
    public interface IDeleteIngredientContext
    {
        Task<bool> Execute(Guid id);
    }
} 