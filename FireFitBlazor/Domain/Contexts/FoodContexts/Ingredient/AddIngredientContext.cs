using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;
using FireFitBlazor.Domain.ContextInterfaces.FoodContexts.Ingredient;


public class AddIngredientContext : IAddIngredientContext
{
    private readonly IAddIngredientGateway _addIngredientGateway;

    public AddIngredientContext(IAddIngredientGateway addIngredientGateway)
    {
        _addIngredientGateway = addIngredientGateway ?? throw new ArgumentNullException(nameof(addIngredientGateway));
    }

    public async Task<bool> Execute(Ingredient ingredient)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient), Messages.Error_NullEntity);
        if (string.IsNullOrWhiteSpace(ingredient.Name))
            throw new ArgumentException(Messages.Error_EmptyName, nameof(ingredient.Name));
        return await _addIngredientGateway.AddAsync(ingredient);
    }
}
