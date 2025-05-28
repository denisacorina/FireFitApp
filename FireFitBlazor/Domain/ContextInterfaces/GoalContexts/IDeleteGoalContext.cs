namespace FireFitBlazor.Domain.ContextInterfaces.GoalContexts
{
    public interface IDeleteGoalContext
    {
        Task<bool> DeleteGoalAsync(Guid goalId);
    }
} 