using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.UserProgress;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;



public class AddUserProgressGateway : BaseGateway<UserProgress>, IAddUserProgressGateway
{
    public AddUserProgressGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> AddAsync(UserProgress userProgress)
    {
        try
        {
            // Check if the user exists
            var userExists = await _context.Set<User>()
                .AnyAsync(u => u.UserId == userProgress.UserId);

            if (!userExists)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            // Check for duplicate progress on the same date
            var duplicateExists = await _dbSet
                .AnyAsync(p => p.UserId == userProgress.UserId && p.UpdatedAt == userProgress.UpdatedAt);

            if (duplicateExists)
            {
                throw new InvalidOperationException(Messages.Error_DuplicateDate);
            }

            await _dbSet.AddAsync(userProgress);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
