using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.FoodLog;


public class AddFoodLogContext : IAddFoodLogContext
{
    private readonly IAddFoodLogGateway _addFoodLogGateway;

    public AddFoodLogContext(IAddFoodLogGateway addFoodLogGateway)
    {
        _addFoodLogGateway = addFoodLogGateway ?? throw new ArgumentNullException(nameof(addFoodLogGateway));
    }

    public async Task<bool> Execute(FoodLog foodLog)
    {
        if (foodLog == null)
            throw new ArgumentNullException(nameof(foodLog), Messages.Error_NullEntity);

    
        return await _addFoodLogGateway.AddAsync(foodLog);
    }
}
