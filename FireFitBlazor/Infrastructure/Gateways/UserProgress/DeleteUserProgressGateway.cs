using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.UserProgress;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;

public class DeleteUserProgressGateway : BaseGateway<UserProgress>, IDeleteUserProgressGateway
{
    public DeleteUserProgressGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            var progress = await _dbSet
                .FirstOrDefaultAsync(p => p.UserId == id);

            if (progress == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            _dbSet.Remove(progress);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
