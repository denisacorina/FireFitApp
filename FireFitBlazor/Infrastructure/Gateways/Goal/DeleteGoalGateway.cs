using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Goal;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;

public class DeleteGoalGateway : BaseGateway<Goal>, IDeleteGoalGateway
{
    public DeleteGoalGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var goal = await _dbSet
                .FirstOrDefaultAsync(g => g.GoalId == id);

            if (goal == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            _dbSet.Remove(goal);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
