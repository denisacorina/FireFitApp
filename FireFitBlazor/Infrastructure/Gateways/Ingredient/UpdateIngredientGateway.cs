using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;


public class UpdateIngredientGateway : BaseGateway<Ingredient>, IUpdateIngredientGateway
{
    public UpdateIngredientGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> UpdateAsync(Ingredient ingredient)
    {
        try
        {
            var existing = await _dbSet.FirstOrDefaultAsync(i => i.IngredientId == ingredient.IngredientId);
            if (existing == null)
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            if (existing.Name.ToLower() != ingredient.Name.ToLower())
            {
                var duplicateExists = await _dbSet.AnyAsync(i => i.Name.ToLower() == ingredient.Name.ToLower() && i.IngredientId != ingredient.IngredientId);
                if (duplicateExists)
                    throw new InvalidOperationException(Messages.Error_DuplicateIngredient);
            }
            _context.Entry(existing).CurrentValues.SetValues(ingredient);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
