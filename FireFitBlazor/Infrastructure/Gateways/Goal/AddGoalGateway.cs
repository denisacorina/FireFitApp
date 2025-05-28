using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Goal;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;


public class AddGoalGateway : BaseGateway<Goal>, IAddGoalGateway
{
    public AddGoalGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> AddAsync(Goal goal)
    {
        try
        {
            // Check for duplicate goal type for the user
            var duplicateExists = await _dbSet
                .AnyAsync(g => g.Type == goal.Type && g.UserId == goal.UserId);

            if (duplicateExists)
            {
                throw new InvalidOperationException(Messages.Error_DuplicateGoal);
            }

            // Verify user exists
            var userExists = await _context.Set<User>()
                .AnyAsync(u => u.UserId == goal.UserId);

            if (!userExists)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            await _dbSet.AddAsync(goal);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
