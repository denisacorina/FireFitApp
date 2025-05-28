using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeRecommendation
{
    public class NutritionCache
    {
        private readonly Dictionary<string, IngredientNutrition> _cache = new();

        public IngredientNutrition GetOrAdd(string ingredient, Func<IngredientNutrition> fetchFunction)
        {
            if (_cache.TryGetValue(ingredient.ToLower(), out var cached))
                return cached;

            var result = fetchFunction();
            _cache[ingredient.ToLower()] = result;
            return result;
        }
    }

}
