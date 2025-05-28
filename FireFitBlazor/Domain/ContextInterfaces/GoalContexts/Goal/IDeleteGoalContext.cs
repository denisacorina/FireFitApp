using System.Threading.Tasks;

namespace FireFitBlazor.Domain.ContextInterfaces.GoalContexts.Goal
{
    public interface IDeleteGoalContext
    {
        Task<bool> Execute(int id);
    }
} 