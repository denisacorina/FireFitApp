using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IGetRecipeGateway
{
    Task<Recipe> GetByIdAsync(Guid id);
    Task<Recipe> GetByNameAsync(string name);
}
