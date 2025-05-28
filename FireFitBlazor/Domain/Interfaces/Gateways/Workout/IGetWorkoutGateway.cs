using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.Interfaces.Gateways.Workout
{
    public interface IGetWorkoutGateway
    {
        Task<WorkoutSession> GetByIdAsync(string id);
       
    }
} 