using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.Interfaces.Gateways
{
    public interface IWorkoutSessionGateway
    {
        Task<bool> AddAsync(WorkoutSession session);
        Task<WorkoutSession> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(WorkoutSession session);
        Task<bool> DeleteAsync(int id);
    }
} 