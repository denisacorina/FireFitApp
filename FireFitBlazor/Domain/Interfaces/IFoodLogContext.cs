using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.Interfaces
{
    public interface IFoodLogContext
    {
        Task<FoodLog> CreateFoodLogAsync(FoodLog foodLog);
        Task<IEnumerable<FoodLog>> GetUserFoodLogsAsync(string userId);
        Task<bool> DeleteFoodLogAsync(Guid foodLogId);
        Task<IEnumerable<FoodLog>> SearchFoodAsync(string query);
    }
} 