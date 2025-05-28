using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.ContextInterfaces.ProgressContexts
{
    public interface IUpdateUserProgressContext
    {
        Task<UserProgress> UpdateUserProgressAsync(UserProgress progress);
    }
} 