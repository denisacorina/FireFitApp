using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Workout;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;

namespace FireFitBlazor.Infrastructure.Gateways.Workout
{
    public class DeleteWorkoutGateway : BaseGateway<WorkoutSession>, IDeleteWorkoutGateway
    {
        public DeleteWorkoutGateway(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var workout = await _dbSet
                    .Include(w => w.Exercises)
                    .FirstOrDefaultAsync();

                if (workout == null)
                {
                    throw new InvalidOperationException(Messages.Error_EntityNotFound);
                }

                _dbSet.Remove(workout);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
            }
        }
    }
} 