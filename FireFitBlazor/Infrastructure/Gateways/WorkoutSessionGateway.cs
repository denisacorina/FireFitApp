using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways;
using FireFitBlazor.Infrastructure.Data;

namespace FireFitBlazor.Infrastructure.Gateways
{
    public class WorkoutSessionGateway : BaseGateway<WorkoutSession>, IWorkoutSessionGateway
    {
        public WorkoutSessionGateway(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<WorkoutSession> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(w => w.Exercises)
                .FirstOrDefaultAsync(w => w.SessionId == id);
        }

    
    }
} 