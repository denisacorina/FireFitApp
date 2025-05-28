using System.Threading.Tasks;

namespace FireFitBlazor.Domain.ContextInterfaces.UserContexts.User
{
    public interface IDeleteUserContext
    {
        Task<bool> Execute(int id);
    }
} 