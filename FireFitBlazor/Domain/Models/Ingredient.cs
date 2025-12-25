
using System.ComponentModel.DataAnnotations;
using FireFitBlazor.Domain.ValueObjects;

namespace FireFitBlazor.Domain.Models;

public sealed class Ingredient
{
    [Key]
    public Guid IngredientId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public NutritionalInfo Nutrition { get; set; }

    public static Ingredient Create(string name, float calories, float proteins, float carbs, float fats)
    {
        return new Ingredient
        {
            IngredientId = Guid.NewGuid(),
            Name = name,
            Nutrition = NutritionalInfo.Create(calories, proteins, carbs, fats)
        };
    }

}
