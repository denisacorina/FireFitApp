using FuzzySharp;

namespace RecipeRecommendation
{
    public static class IngredientMatcher
    {
        public static string Match(string input, List<string> knownIngredients, int threshold = 80)
        {
            var best = Process.ExtractOne(input, knownIngredients);
            return best.Score >= threshold ? best.Value : null;
        }
    }
}
