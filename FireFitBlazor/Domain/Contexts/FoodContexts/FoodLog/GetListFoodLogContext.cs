using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.FoodLog;


public class GetListFoodLogContext : IGetListFoodLogContext
{
    private readonly IGetListFoodLogGateway _getListFoodLogGateway;

    public GetListFoodLogContext(IGetListFoodLogGateway getListFoodLogGateway)
    {
        _getListFoodLogGateway = getListFoodLogGateway ?? throw new ArgumentNullException(nameof(getListFoodLogGateway));
    }

    public async Task<IEnumerable<FoodLog>> ExecuteByCalorieLog(int calorieLogId)
    {
        if (calorieLogId <= 0)
            throw new ArgumentException(Messages.Error_InvalidCalorieLogId, nameof(calorieLogId));

        return await _getListFoodLogGateway.GetByCalorieLogIdAsync(calorieLogId);
    }

    public async Task<IEnumerable<FoodLog>> ExecuteByFood(int foodId)
    {
        if (foodId <= 0)
            throw new ArgumentException(Messages.Error_InvalidFoodId, nameof(foodId));

        return await _getListFoodLogGateway.GetByFoodIdAsync(foodId);
    }
}
