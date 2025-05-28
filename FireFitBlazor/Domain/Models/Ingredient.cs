
using FireFitBlazor.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.Models
{
    public sealed class Ingredient
    {
        [Key]
        public Guid IngredientId { get; set; }
        public string Name { get; set; }
        public NutritionalInfo Nutrition { get; set; }
        public bool ContainsAnimalProducts { get; set; }
        public bool ContainsGluten { get; set; }
        public bool ContainsLactose { get; set; }


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
}
