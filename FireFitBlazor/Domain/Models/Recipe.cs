using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FireFitBlazor.Domain.ValueObjects;

namespace FireFitBlazor.Domain.Models
{
    public sealed class Recipe
    {
        [Key]
        public Guid RecipeId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new();
        public string Instructions { get; set; }
        public NutritionalInfo Nutrition { get; set; }
        public DateTime CreatedAt { get; set; }

        private Recipe()
        {
        }

        public static Recipe CreateForGeneratedRecipe(string userId, string title, List<Ingredient> ingredients,
            string instructions)
        {
            return new Recipe
            {
                RecipeId = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Ingredients = ingredients,
                Instructions = instructions,
                Nutrition = NutritionalInfo.Calculate(ingredients),
                CreatedAt = DateTime.UtcNow
            };
        }

        public static Recipe CreateForLoggedRecipe(string userId, string title, List<Ingredient> ingredients,
            string instructions, float calories, float proteins, float carbs, float fats)
        {
            return new Recipe
            {
                RecipeId = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Ingredients = ingredients,
                Instructions = instructions,
                Nutrition = NutritionalInfo.Create(calories, proteins, carbs, fats),
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string title, List<Ingredient> ingredients, string instructions)
        {
            Title = title;
            Ingredients = ingredients;
            Instructions = instructions;
            Nutrition = NutritionalInfo.Calculate(ingredients);
        }

        public void AddIngredient(Ingredient ingredient)
        {
            Ingredients.Add(ingredient);
            Nutrition = NutritionalInfo.Calculate(Ingredients);
        }

        public void RemoveIngredient(string ingredientName)
        {
            var ingredient =
                Ingredients.FirstOrDefault(i => i.Name.Equals(ingredientName, StringComparison.OrdinalIgnoreCase));
            if (ingredient != null)
            {
                Ingredients.Remove(ingredient);
                Nutrition = NutritionalInfo.Calculate(Ingredients);
            }
        }

        public void SubstituteIngredient(string oldIngredientName, Ingredient newIngredient)
        {
            RemoveIngredient(oldIngredientName);
            AddIngredient(newIngredient);
        }
    }
}
