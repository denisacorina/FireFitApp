using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Goal;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;
using FireFitBlazor.Domain.Enums;


public class GetGoalGateway : BaseGateway<Goal>, IGetGoalGateway
{
    public GetGoalGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Goal> GetByIdAsync(Guid id)
    {
        try
        {
            var goal = await _dbSet
                .FirstOrDefaultAsync(g => g.GoalId == id);

            if (goal == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            return goal;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }

    public async Task<Goal> GetByTypeAsync(GoalType type, string userId)
    {
        try
        {
            var goal = await _dbSet
                .FirstOrDefaultAsync(g => g.Type == type && g.UserId == userId);

            if (goal == null)
            {
                throw new InvalidOperationException(Messages.Error_EntityNotFound);
            }

            return goal;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
