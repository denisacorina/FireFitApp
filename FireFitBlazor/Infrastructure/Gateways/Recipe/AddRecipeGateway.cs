using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Recipe;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;
using FireFitBlazor.Application.Services;


public class AddRecipeGateway : BaseGateway<Recipe>, IAddRecipeGateway
{
    public AddRecipeGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> AddAsync(Recipe recipe)
    {
        try
        {
            // Check for duplicate recipe name
            var duplicateExists = await _dbSet
                .AnyAsync(r => r.Title == recipe.Title);

            if (duplicateExists)
            {
                throw new InvalidOperationException(Messages.Error_DuplicateRecipe);
            }

            // Validate all ingredients exist
            foreach (var ingredient in recipe.Ingredients)
            {
                var foodExists = await _context.Set<FoodItem>()
                    .AnyAsync(f => f.Id == ingredient.IngredientId);

                if (!foodExists)
                {
                    throw new InvalidOperationException(Messages.Error_EntityNotFound);
                }
            }

            await _dbSet.AddAsync(recipe);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
