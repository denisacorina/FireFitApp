using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IUpdateIngredientGateway
{
    Task<bool> UpdateAsync(Ingredient ingredient);
}
