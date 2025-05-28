using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;


public class GetFoodLogGateway : BaseGateway<FoodLog>, IGetFoodLogGateway
{
    public GetFoodLogGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<FoodLog> GetByIdAsync(Guid id)
    {
        try
        {
            var foodLog = await _dbSet
                .FirstOrDefaultAsync(f => f.FoodLogId == id);

            if (foodLog == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            return foodLog;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }

}
