using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.UserProgress;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;

public class GetUserProgressGateway : BaseGateway<UserProgress>, IGetUserProgressGateway
{
    public GetUserProgressGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserProgress> GetByIdAsync(string id)
    {
        try
        {
            var progress = await _dbSet
                .FirstOrDefaultAsync(p => p.UserId == id);

            if (progress == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            return progress;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }

    public async Task<UserProgress> GetByUserIdAndDateAsync(string userId, DateTime date)
    {
        try
        {
            var progress = await _dbSet
                .FirstOrDefaultAsync(p => p.UserId == userId && p.UpdatedAt == date.Date);

            if (progress == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            return progress;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
