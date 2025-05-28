using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.CalorieLog;
using FireFitBlazor.Domain.Interfaces.Gateways.CalorieLog;
using FireFitBlazor.Domain.Resources;


public class AddCalorieLogContext : IAddCalorieLogContext
{
    private readonly IAddCalorieLogGateway _addCalorieLogGateway;

    public AddCalorieLogContext(IAddCalorieLogGateway addCalorieLogGateway)
    {
        _addCalorieLogGateway = addCalorieLogGateway;
    }

    public async Task<bool> Execute(CalorieLog calorieLog)
    {
        if (calorieLog == null)
        {
            throw new ArgumentNullException(nameof(calorieLog), Messages.Error_NullEntity);
        }

        if (calorieLog.UserId == string.Empty)
        {
            throw new ArgumentException(Messages.Error_InvalidId, nameof(calorieLog.UserId));
        }

        if (calorieLog.Date == default)
        {
            throw new ArgumentException(Messages.Error_InvalidDate, nameof(calorieLog.Date));
        }

        var result = await _addCalorieLogGateway.AddAsync(calorieLog);
        if (!result)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }

        return true;
    }
}
