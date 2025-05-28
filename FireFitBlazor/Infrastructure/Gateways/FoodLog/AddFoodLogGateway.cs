using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;

public class AddFoodLogGateway : BaseGateway<FoodLog>, IAddFoodLogGateway
{
    public AddFoodLogGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> AddAsync(FoodLog foodLog)
    {
        try
        {
            await _dbSet.AddAsync(foodLog);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
