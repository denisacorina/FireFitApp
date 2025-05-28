using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.Ingredient;

namespace FireFitBlazor.Domain.Contexts.FoodContexts.Ingredient
{
    public class DeleteIngredientContext : IDeleteIngredientContext
    {
        private readonly IDeleteIngredientGateway _deleteIngredientGateway;

        public DeleteIngredientContext(IDeleteIngredientGateway deleteIngredientGateway)
        {
            _deleteIngredientGateway = deleteIngredientGateway ?? throw new ArgumentNullException(nameof(deleteIngredientGateway));
        }

        public async Task<bool> Execute(Guid id)
        {
          
            return await _deleteIngredientGateway.DeleteAsync(id);
        }
    }
} 