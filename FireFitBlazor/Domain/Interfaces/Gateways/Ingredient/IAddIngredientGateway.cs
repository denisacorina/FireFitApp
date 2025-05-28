using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IAddIngredientGateway
{
    Task<bool> AddAsync(Ingredient ingredient);
}
