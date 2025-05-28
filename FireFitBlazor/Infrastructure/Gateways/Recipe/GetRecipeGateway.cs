using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Recipe;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;

public class GetRecipeGateway : BaseGateway<Recipe>, IGetRecipeGateway
{
    public GetRecipeGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Recipe> GetByIdAsync(Guid id)
    {
        try
        {
            var recipe = await _dbSet
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            return recipe;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }

    public async Task<Recipe> GetByNameAsync(string name)
    {
        try
        {
            var recipe = await _dbSet
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Title == name);

            if (recipe == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            return recipe;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
