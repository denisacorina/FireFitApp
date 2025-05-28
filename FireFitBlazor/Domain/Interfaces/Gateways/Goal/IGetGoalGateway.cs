using System.Threading.Tasks;
using FireFitBlazor.Domain.Enums;
using FireFitBlazor.Domain.Models;

public interface IGetGoalGateway
{
    Task<Goal> GetByIdAsync(Guid id);
    Task<Goal> GetByTypeAsync(GoalType type, string userId);
}
