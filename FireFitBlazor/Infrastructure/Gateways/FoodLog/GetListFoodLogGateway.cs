using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;


public class GetListFoodLogGateway : BaseGateway<FoodLog>, IGetListFoodLogGateway
{
    public GetListFoodLogGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<FoodLog>> GetByCalorieLogIdAsync(int calorieLogId)
    {
        try
        {
            var foodLogs = await _dbSet
                .Include(f => f.NutritionalInfo)
                .OrderBy(f => f.FoodName)
                .ToListAsync();

            if (!foodLogs.Any())
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            return foodLogs;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }

    public async Task<IEnumerable<FoodLog>> GetByFoodIdAsync(int foodId)
    {
        try
        {
            var foodLogs = await _dbSet
                .OrderByDescending(f => f.NutritionalInfo)
                .ToListAsync();

            if (!foodLogs.Any())
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            return foodLogs;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
