using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.Interfaces.Gateways
{
    public interface IExerciseLogGateway
    {
        Task<bool> AddAsync(ExerciseLog log);
        Task<ExerciseLog> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(ExerciseLog log);
    }
} 