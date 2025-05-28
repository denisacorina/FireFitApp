using FireFitBlazor.Domain.Models;


    public interface ICreateGoalContext
    {
        Task<Goal> CreateGoalAsync(Goal goal);
    }
