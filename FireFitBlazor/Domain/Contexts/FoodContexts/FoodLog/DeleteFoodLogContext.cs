using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.FoodLog;

namespace FireFitBlazor.Domain.Contexts.FoodContexts.FoodLog
{
    public class DeleteFoodLogContext : IDeleteFoodLogContext
    {
        private readonly IDeleteFoodLogGateway _deleteFoodLogGateway;

        public DeleteFoodLogContext(IDeleteFoodLogGateway deleteFoodLogGateway)
        {
            _deleteFoodLogGateway = deleteFoodLogGateway ?? throw new ArgumentNullException(nameof(deleteFoodLogGateway));
        }

        public async Task<bool> Execute(Guid id)
        {
           
            return await _deleteFoodLogGateway.DeleteAsync(id);
        }
    }
} 