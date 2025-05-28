using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Workout;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Domain.Resources;

namespace FireFitBlazor.Infrastructure.Gateways.Workout
{
    public class GetWorkoutGateway : BaseGateway<WorkoutSession>, IGetWorkoutGateway
    {
        public GetWorkoutGateway(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<WorkoutSession> GetByIdAsync(string id)
        {
            try
            {
                var workout = await _dbSet
                    .Include(w => w.Exercises)
                    .FirstOrDefaultAsync(w => w.UserId == id);

                if (workout == null)
                {
                    throw new InvalidOperationException(Messages.Error_EntityNotFound);
                }

                return workout;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(Messages.Error_DatabaseOperationFailed);
            }
        }

       
    }
} 