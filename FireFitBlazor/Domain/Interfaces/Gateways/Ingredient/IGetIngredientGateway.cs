using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IGetIngredientGateway
{
    Task<Ingredient> GetByIdAsync(Guid id);
    Task<Ingredient> GetByNameAsync(string name);
}
