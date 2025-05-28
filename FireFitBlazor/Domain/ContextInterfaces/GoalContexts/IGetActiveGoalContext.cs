using FireFitBlazor.Domain.Models;


    public interface IGetActiveGoalContext
    {
        Task<Goal> GetActiveGoalAsync(string userId);
    }
