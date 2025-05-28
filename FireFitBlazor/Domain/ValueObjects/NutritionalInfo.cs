using FireFitBlazor.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.ValueObjects
{
    public sealed class NutritionalInfo
    {
        public float Calories { get; init; }
        public float Proteins { get; init; }
        public float Carbs { get; init; }
        public float Fats { get; init; }

        public NutritionalInfo()
        {
           
        }

        public NutritionalInfo(float calories, float proteins, float carbs, float fats)
        {
            if (calories < 0 || proteins < 0 || carbs < 0 || fats < 0)
                throw new ArgumentException("Nutritional values cannot be negative.");

            Calories = calories;
            Proteins = proteins;
            Carbs = carbs;
            Fats = fats;
        }

        public static NutritionalInfo Create(float calories, float proteins, float carbs, float fats)
        {
            return new NutritionalInfo(calories, proteins, carbs, fats);
        }

        public static NutritionalInfo Calculate(List<Ingredient> ingredients)
        {
            return new NutritionalInfo(
                ingredients.Sum(i => i.Nutrition.Calories),
                ingredients.Sum(i => i.Nutrition.Proteins),
                ingredients.Sum(i => i.Nutrition.Carbs),
                ingredients.Sum(i => i.Nutrition.Fats));
        }

        public static NutritionalInfo Zero() => new(0, 0, 0, 0);
    }
}
