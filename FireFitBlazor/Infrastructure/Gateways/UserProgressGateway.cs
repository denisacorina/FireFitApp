using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Infrastructure.Gateways;


public class UserProgressGateway : BaseGateway<UserProgress>, IUserProgressGateway
    {
        public UserProgressGateway(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserProgress> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(p => p.Achievements)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
