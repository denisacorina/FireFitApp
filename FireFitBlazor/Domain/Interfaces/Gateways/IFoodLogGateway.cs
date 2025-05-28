using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.Interfaces.Gateways
{
    public interface IFoodLogGateway
    {
        Task<bool> AddAsync(FoodLog log);
        Task<FoodLog> GetByIdAsync(Guid id);
        Task<List<FoodLog>> GetRecentByUserIdAsync(string userId, int days);
        Task<bool> UpdateAsync(FoodLog log);
        Task<bool> DeleteAsync(Guid id);
    }
} 