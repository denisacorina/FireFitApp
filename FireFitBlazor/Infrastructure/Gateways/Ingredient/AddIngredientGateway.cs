using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;


public class AddIngredientGateway : BaseGateway<Ingredient>, IAddIngredientGateway
{
    public AddIngredientGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> AddAsync(Ingredient ingredient)
    {
        try
        {
            var duplicateExists = await _dbSet.AnyAsync(i => i.Name.ToLower() == ingredient.Name.ToLower());
            if (duplicateExists)
                throw new InvalidOperationException(Messages.Error_DuplicateIngredient);
            await _dbSet.AddAsync(ingredient);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
