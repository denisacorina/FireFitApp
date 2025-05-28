using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.CalorieLog;
using FireFitBlazor.Domain.Interfaces.Gateways.CalorieLog;
using FireFitBlazor.Domain.Resources;


public class GetCalorieLogContext : IGetCalorieLogContext
{
    private readonly IGetCalorieLogGateway _getCalorieLogGateway;

    public GetCalorieLogContext(IGetCalorieLogGateway getCalorieLogGateway)
    {
        _getCalorieLogGateway = getCalorieLogGateway;
    }

    public async Task<CalorieLog> Execute(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(Messages.Error_InvalidId, nameof(id));
        }

        var calorieLog = await _getCalorieLogGateway.GetByIdAsync(id);
        if (calorieLog == null)
        {
            throw new InvalidOperationException(Messages.Error_EntityNotFound);
        }

        return calorieLog;
    }

    public async Task<CalorieLog> ExecuteByUserAndDate(int userId, DateTime date)
    {
        if (userId <= 0)
        {
            throw new ArgumentException(Messages.Error_InvalidId, nameof(userId));
        }

        var calorieLog = await _getCalorieLogGateway.GetByUserIdAndDateAsync(userId, date);
        if (calorieLog == null)
        {
            throw new InvalidOperationException(Messages.Error_EntityNotFound);
        }

        return calorieLog;
    }
}
