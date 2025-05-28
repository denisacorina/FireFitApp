using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.FoodLog;


public class UpdateFoodLogContext : IUpdateFoodLogContext
{
    private readonly IUpdateFoodLogGateway _updateFoodLogGateway;

    public UpdateFoodLogContext(IUpdateFoodLogGateway updateFoodLogGateway)
    {
        _updateFoodLogGateway = updateFoodLogGateway ?? throw new ArgumentNullException(nameof(updateFoodLogGateway));
    }

    public async Task<bool> Execute(FoodLog foodLog)
    {
        if (foodLog == null)
            throw new ArgumentNullException(nameof(foodLog), Messages.Error_NullEntity);

        

        return await _updateFoodLogGateway.UpdateAsync(foodLog);
    }
}
