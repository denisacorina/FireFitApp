using System.Threading.Tasks;

namespace FireFitBlazor.Domain.Interfaces.Gateways.Workout
{
    public interface IDeleteWorkoutGateway
    {
        Task<bool> DeleteAsync(int id);
    }
} 