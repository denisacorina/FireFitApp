using FireFitBlazor.Domain.Models;


    public interface IGetUserGoalsContext
    {
        Task<IEnumerable<Goal>> GetUserGoalsAsync(string userId);
    }
