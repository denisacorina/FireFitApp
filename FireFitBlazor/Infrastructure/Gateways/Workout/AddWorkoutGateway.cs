using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Workout;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Infrastructure.Gateways;


public class AddWorkoutGateway : BaseGateway<WorkoutSession>, IAddWorkoutGateway
{
    public AddWorkoutGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> AddAsync(WorkoutSession workout)
    {
        try
        {
            await _dbSet.AddAsync(workout);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
        }
    }
}
