using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.ContextInterfaces.WorkoutContexts.Workout
{
    public interface IGetWorkoutContext
    {
        Task<WorkoutSession> Execute(Guid id);
    }
} 