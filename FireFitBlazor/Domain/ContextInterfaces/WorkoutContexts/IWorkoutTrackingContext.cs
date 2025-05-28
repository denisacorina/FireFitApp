using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.ContextInterfaces.WorkoutContexts
{
    public interface IWorkoutTrackingContext
    {
        Task<bool> Execute(WorkoutSession request);
    }
} 