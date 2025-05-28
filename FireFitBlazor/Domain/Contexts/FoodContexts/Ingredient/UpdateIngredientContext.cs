using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.Ingredient;


public class UpdateIngredientContext : IUpdateIngredientContext
{
    private readonly IUpdateIngredientGateway _updateIngredientGateway;

    public UpdateIngredientContext(IUpdateIngredientGateway updateIngredientGateway)
    {
        _updateIngredientGateway = updateIngredientGateway ?? throw new ArgumentNullException(nameof(updateIngredientGateway));
    }

    public async Task<bool> Execute(Ingredient ingredient)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient), Messages.Error_NullEntity);
     
        if (string.IsNullOrWhiteSpace(ingredient.Name))
            throw new ArgumentException(Messages.Error_EmptyName, nameof(ingredient.Name));
        return await _updateIngredientGateway.UpdateAsync(ingredient);
    }
}
