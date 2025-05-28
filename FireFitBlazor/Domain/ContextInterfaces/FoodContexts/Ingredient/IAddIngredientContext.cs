using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IAddIngredientContext
{
    Task<bool> Execute(Ingredient ingredient);
}
