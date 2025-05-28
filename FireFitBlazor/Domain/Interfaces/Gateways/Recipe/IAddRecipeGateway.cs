using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IAddRecipeGateway
{
    Task<bool> AddAsync(Recipe recipe);
}
