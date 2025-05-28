using System.Threading.Tasks;

namespace FireFitBlazor.Domain.ContextInterfaces.WorkoutContexts.Workout
{
    public interface IDeleteWorkoutContext
    {
        Task<bool> Execute(int id);
    }
} 