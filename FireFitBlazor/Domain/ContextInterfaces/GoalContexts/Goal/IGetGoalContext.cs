using System.Threading.Tasks;
using FireFitBlazor.Domain.Enums;
using FireFitBlazor.Domain.Models;


public interface IGetGoalContext
{
    Task<Goal> Execute(Guid id);
    Task<Goal> ExecuteByType(GoalType type, string userId);
}
