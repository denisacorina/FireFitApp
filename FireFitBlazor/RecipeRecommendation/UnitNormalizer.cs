using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeRecommendation
{
    public static class UnitNormalizer
    {
        public static decimal NormalizeToGrams(decimal quantity, string unit)
        {
            unit = unit.ToLower().Trim();

            return unit switch
            {
                "g" or "gram" or "grams" => quantity,
                "kg" or "kilogram" or "kilograms" => quantity * 1000,
                "mg" or "milligram" or "milligrams" => quantity / 1000,
                "lb" or "pound" or "pounds" => quantity * 453.592m,
                "oz" or "ounce" or "ounces" => quantity * 28.3495m,
                "cup" => quantity * 240,      // general estimate
                "tbsp" or "tablespoon" => quantity * 15,
                "tsp" or "teaspoon" => quantity * 5,
                _ => quantity // Unknown → return as-is (maybe already normalized)
            };
        }

        public static decimal NormalizeToGramsJson(string quantity, string unit)
        {
            if (string.IsNullOrEmpty(quantity)) return 0;

            // First try to match pattern like "2 shells (60g)"
            var match = Regex.Match(quantity, @"(\d+)\s*(?:shells?|cloves?)?\s*\((\d+)g\)");
            if (match.Success)
            {
                var count = decimal.Parse(match.Groups[1].Value);
                var totalGrams = decimal.Parse(match.Groups[2].Value);
                return totalGrams / count;
            }

            // Try to extract just the number part
            var numberMatch = Regex.Match(quantity, @"(\d+(?:\.\d+)?)");
            if (numberMatch.Success)
            {
                var number = decimal.Parse(numberMatch.Value);

                // Apply unit conversions if needed
                switch (unit?.ToLower())
                {
                    case "kg":
                        return number * 1000;
                    case "mg":
                        return number / 1000;
                    case "oz":
                        return number * 28.35m;
                    case "lb":
                        return number * 453.592m;
                    case "cup":
                        return number * 236.588m;
                    case "tbsp":
                        return number * 14.787m;
                    case "tsp":
                        return number * 4.929m;
                    default: // Assume grams if no unit or unknown unit
                        return number;
                }
            }

            return 0; // Return 0 if no valid number found
        }


    }
}
