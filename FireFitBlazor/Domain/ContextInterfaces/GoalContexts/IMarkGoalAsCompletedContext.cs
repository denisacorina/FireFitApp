using FireFitBlazor.Domain.Models;



public interface IMarkGoalAsCompletedContext
{
    Task<Goal> MarkGoalAsCompletedAsync(Guid goalId);
}
