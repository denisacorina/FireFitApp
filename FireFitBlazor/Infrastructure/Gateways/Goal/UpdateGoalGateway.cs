using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Goal;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;

public class UpdateGoalGateway : BaseGateway<Goal>, IUpdateGoalGateway
{
    public UpdateGoalGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> UpdateAsync(Goal goal)
    {
        try
        {
            var existing = await _dbSet
                .FirstOrDefaultAsync(g => g.GoalId == goal.GoalId);

            if (existing == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            // Check if the user exists
            var userExists = await _context.Set<User>()
                .AnyAsync(u => u.UserId == goal.UserId);

            if (!userExists)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            // Check for duplicate goal type if type changed
            if (existing.Type != goal.Type)
            {
                var duplicateExists = await _dbSet
                    .AnyAsync(g => g.UserId == goal.UserId && 
                                 g.Type == goal.Type && 
                                 g.GoalId != goal.GoalId);

                if (duplicateExists)
                {
                    throw new InvalidOperationException(Messages.Error_DuplicateGoal);
                }
            }

            _context.Entry(existing).CurrentValues.SetValues(goal);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
