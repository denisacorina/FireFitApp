using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IGetFoodLogGateway
{
    Task<FoodLog> GetByIdAsync(Guid id);
}
