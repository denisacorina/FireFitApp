using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;


public class UpdateFoodLogGateway : BaseGateway<FoodLog>, IUpdateFoodLogGateway
{
    public UpdateFoodLogGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> UpdateAsync(FoodLog foodLog)
    {
        try
        {
            var existingFoodLog = await _dbSet
                .Include(f => f.NutritionalInfo)
                .FirstOrDefaultAsync(f => f.FoodLogId == foodLog.FoodLogId);

            if (existingFoodLog == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }


            _context.Entry(existingFoodLog).CurrentValues.SetValues(foodLog);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
