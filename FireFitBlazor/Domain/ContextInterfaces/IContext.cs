using System.Threading.Tasks;

namespace FireFitBlazor.Domain.ContextInterfaces
{
    public interface IContext<TRequest, TResponse>
    {
        Task<TResponse> Execute(TRequest request);
    }

    public interface IContext<TResponse>
    {
        Task<TResponse> Execute();
    }
} 