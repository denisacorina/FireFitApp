using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.Ingredient;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;

public class GetListIngredientContext : IGetListIngredientContext
{
    private readonly IGetListIngredientGateway _getListIngredientGateway;

    public GetListIngredientContext(IGetListIngredientGateway getListIngredientGateway)
    {
        _getListIngredientGateway = getListIngredientGateway;
    }

    public async Task<IEnumerable<Ingredient>> Execute()
    {
        return await _getListIngredientGateway.GetAllAsync();
    }

  
}
