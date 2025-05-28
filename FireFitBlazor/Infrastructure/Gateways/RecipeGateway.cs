using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Infrastructure.Gateways;

public class RecipeGateway : BaseGateway<Recipe>, IRecipeGateway
{
    public RecipeGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Ingredients)
            .OrderBy(r => r.Title)
            .ToListAsync();
    }

    public async Task<Recipe> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.RecipeId == id);
    }

    public async Task<List<Recipe>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(r => r.Ingredients)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.Title)
            .ToListAsync();
    }

}
