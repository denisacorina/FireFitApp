using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Infrastructure.Gateways;

public class UserGateway : BaseGateway<User>, IUserGateway
{
    public UserGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User> GetByIdAsync(string id)
    {
        return await _dbSet
            .Include(u => u.Progress)
            .Include(u => u.Preferences)
            .FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _dbSet
             .Include(u => u.Progress)
            .Include(u => u.Preferences)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}
