using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Infrastructure.Gateways;


public class FoodLogGateway : BaseGateway<FoodLog>, IFoodLogGateway
{
    public FoodLogGateway(ApplicationDbContext context) : base(context)
    {
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<FoodLog> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(f => f.NutritionalInfo)
            .FirstOrDefaultAsync(f => f.FoodLogId == id);
    }

    public async Task<IEnumerable<FoodLog>> GetRecentByUserIdAsync(string userId, int days)
    {
        var cutoffDate = System.DateTime.UtcNow.AddDays(-days);
        return await _dbSet
            .Where(f => f.UserId == userId && f.Timestamp >= cutoffDate)
            .OrderByDescending(f => f.Timestamp)
            .ToListAsync();
    }

    Task<List<FoodLog>> IFoodLogGateway.GetRecentByUserIdAsync(string userId, int days)
    {
        throw new NotImplementedException();
    }
}
