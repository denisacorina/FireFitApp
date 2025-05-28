using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;



public interface IUpdateIngredientContext
{
    Task<bool> Execute(Ingredient ingredient);
}
