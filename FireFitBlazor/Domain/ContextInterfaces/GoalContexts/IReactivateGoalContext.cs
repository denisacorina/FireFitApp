using FireFitBlazor.Domain.Models;


    public interface IReactivateGoalContext
    {
        Task<Goal> ReactivateGoalAsync(Guid goalId);
    }
