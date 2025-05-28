using System.Threading.Tasks;
using FireFitBlazor.Domain.ContextInterfaces;

namespace FireFitBlazor.Domain.Contexts
{
    public abstract class BaseContext<TRequest, TResponse> : IContext<TRequest, TResponse>
    {
        public abstract Task<TResponse> Execute(TRequest request);
    }

    public abstract class BaseContext<TResponse> : IContext<TResponse>
    {
        public abstract Task<TResponse> Execute();
    }
} 