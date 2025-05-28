using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.Interfaces.Gateways.CalorieLog;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.CalorieLog;

namespace FireFitBlazor.Domain.Contexts.FoodContexts.CalorieLog
{
    public class DeleteCalorieLogContext : IDeleteCalorieLogContext
    {
        private readonly IDeleteCalorieLogGateway _deleteCalorieLogGateway;

        public DeleteCalorieLogContext(IDeleteCalorieLogGateway deleteCalorieLogGateway)
        {
            _deleteCalorieLogGateway = deleteCalorieLogGateway ?? throw new ArgumentNullException(nameof(deleteCalorieLogGateway));
        }

        public async Task<bool> Execute(int id)
        {
            if (id <= 0)
                throw new ArgumentException(Messages.Error_InvalidId, nameof(id));

            return await _deleteCalorieLogGateway.DeleteAsync(id);
        }
    }
} 