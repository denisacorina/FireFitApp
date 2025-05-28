using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;


public class DeleteFoodLogGateway : BaseGateway<FoodLog>, IDeleteFoodLogGateway
{
    public DeleteFoodLogGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var foodLog = await _dbSet
                .FirstOrDefaultAsync(f => f.FoodLogId == id);

            if (foodLog == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            _dbSet.Remove(foodLog);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
