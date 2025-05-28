using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.Interfaces.Gateways.Workout
{
    public interface IAddWorkoutGateway
    {
        Task<bool> AddAsync(WorkoutSession workout);
    }
} 