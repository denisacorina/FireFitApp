using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.Ingredient;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;



public class GetIngredientContext : IGetIngredientContext
{
    private readonly IGetIngredientGateway _getIngredientGateway;

    public GetIngredientContext(IGetIngredientGateway getIngredientGateway)
    {
        _getIngredientGateway = getIngredientGateway;
    }

    public async Task<Ingredient> Execute(Guid id)
    {
       
        return await _getIngredientGateway.GetByIdAsync(id);
    }

    public async Task<Ingredient> ExecuteByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return await _getIngredientGateway.GetByNameAsync(name);
    }
}
