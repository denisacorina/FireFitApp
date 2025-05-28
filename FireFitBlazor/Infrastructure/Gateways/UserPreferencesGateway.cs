using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways;
using FireFitBlazor.Infrastructure.Data;

namespace FireFitBlazor.Infrastructure.Gateways
{
    public class UserPreferencesGateway : BaseGateway<UserPreferences>, IUserPreferencesGateway
    {
        public UserPreferencesGateway(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserPreferences> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
} 