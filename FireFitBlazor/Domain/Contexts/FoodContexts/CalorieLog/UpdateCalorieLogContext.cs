using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.Interfaces.Gateways;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.CalorieLog;
using FireFitBlazor.Domain.Interfaces.Gateways.CalorieLog;



public class UpdateCalorieLogContext : IUpdateCalorieLogContext
{
    private readonly IUpdateCalorieLogGateway _updateCalorieLogGateway;

    public UpdateCalorieLogContext(IUpdateCalorieLogGateway updateCalorieLogGateway)
    {
        _updateCalorieLogGateway = updateCalorieLogGateway ?? throw new ArgumentNullException(nameof(updateCalorieLogGateway));
    }

    public async Task<bool> Execute(CalorieLog calorieLog)
    {
        if (calorieLog == null)
            throw new ArgumentNullException(nameof(calorieLog), Messages.Error_NullEntity);

        if (calorieLog.Date == default)
            throw new ArgumentException(Messages.Error_InvalidDate, nameof(calorieLog.Date));

       

        return await _updateCalorieLogGateway.UpdateAsync(calorieLog);
    }
}
