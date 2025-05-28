using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways;
using FireFitBlazor.Infrastructure.Data;

namespace FireFitBlazor.Infrastructure.Gateways
{
    public class ExerciseLogGateway : BaseGateway<ExerciseLog>, IExerciseLogGateway
    {
        public ExerciseLogGateway(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ExerciseLog> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.ExerciseId == id);
        }
    }
} 