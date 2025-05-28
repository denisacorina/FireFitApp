using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Recipe;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;
using FireFitBlazor.Application.Services;

public class UpdateRecipeGateway : BaseGateway<Recipe>, IUpdateRecipeGateway
{
    public UpdateRecipeGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> UpdateAsync(Recipe recipe)
    {
        try
        {
            var existing = await _dbSet
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.Nutrition)
                .FirstOrDefaultAsync(r => r.RecipeId == recipe.RecipeId);

            if (existing == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            // Check for duplicate recipe name if name changed
            if (existing.Title != recipe.Title)
            {
                var duplicateExists = await _dbSet
                    .AnyAsync(r => r.Title == recipe.Title && r.RecipeId != recipe.RecipeId);

                if (duplicateExists)
                {
                    throw new InvalidOperationException(Messages.Error_DuplicateRecipe);
                }
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

            _context.Entry(existing).CurrentValues.SetValues(recipe);

            // Update ingredients
            existing.Ingredients.Clear();
            foreach (var ingredient in recipe.Ingredients)
            {
                existing.Ingredients.Add(ingredient);
            }

            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
