using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;


public class DeleteIngredientGateway : BaseGateway<Ingredient>, IDeleteIngredientGateway
{
    public DeleteIngredientGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var ingredient = await _dbSet.Include(i => i.Nutrition).FirstOrDefaultAsync(i => i.IngredientId == id);
            if (ingredient == null)
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            _dbSet.Remove(ingredient);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
