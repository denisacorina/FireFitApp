using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.UserProgress;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;

public class UpdateUserProgressGateway : BaseGateway<UserProgress>, IUpdateUserProgressGateway
{
    public UpdateUserProgressGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> UpdateAsync(UserProgress userProgress)
    {
        try
        {
            var existing = await _dbSet
                .FirstOrDefaultAsync(p => p.UserId == userProgress.UserId);

            if (existing == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            // Check if the user exists
            var userExists = await _context.Set<User>()
                .AnyAsync(u => u.UserId == userProgress.UserId);

            if (!userExists)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            // Check for duplicate progress on the same date if date changed
            if (existing.LastMeasurementDate != userProgress.LastMeasurementDate)
            {
                var duplicateExists = await _dbSet
                    .AnyAsync(p => p.UserId == userProgress.UserId && 
                                 p.LastMeasurementDate == userProgress.LastMeasurementDate && 
                                 p.ProgressId != userProgress.ProgressId);

                if (duplicateExists)
                {
                    throw new InvalidOperationException(Messages.Error_DuplicateDate);
                }
            }

            _context.Entry(existing).CurrentValues.SetValues(userProgress);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
