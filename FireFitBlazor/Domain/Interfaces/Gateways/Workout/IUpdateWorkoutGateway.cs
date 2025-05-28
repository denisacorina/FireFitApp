using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IUpdateWorkoutGateway
{
    Task<bool> UpdateAsync(WorkoutSession workout);
}
