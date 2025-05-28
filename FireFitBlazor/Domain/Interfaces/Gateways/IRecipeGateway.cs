using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IRecipeGateway
{
    Task<List<Recipe>> GetAllAsync();
    Task<bool> AddAsync(Recipe recipe);
    Task<bool> UpdateAsync(Recipe recipe);
    Task<bool> DeleteAsync(int id);
}
