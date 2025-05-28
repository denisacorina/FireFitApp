using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.ContextInterfaces.ProgressContexts
{
    public interface IGetUserProgressContext
    {
        Task<UserProgress> GetUserProgressAsync(string userId);
    }
} 