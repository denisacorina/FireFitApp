using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.ContextInterfaces.WorkoutContexts.Workout
{
    public interface IUpdateWorkoutContext
    {
        Task<bool> Execute(WorkoutSession workout);
    }
} 