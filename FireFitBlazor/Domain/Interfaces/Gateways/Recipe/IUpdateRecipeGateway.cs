using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IUpdateRecipeGateway
{
    Task<bool> UpdateAsync(Recipe recipe);
}
