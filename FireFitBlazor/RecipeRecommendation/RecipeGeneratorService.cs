using IntentClassification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Tensorflow.TensorSliceProto.Types;
using static TorchSharp.torch.nn;

namespace RecipeRecommendation
{
    public class RecipeGeneratorService
    {
        private readonly NERPredictor _ner;
        private readonly MLModel1 _intentClassifier;
        private readonly NutritionCache _nutrition;
        private readonly List<IngredientNutrition> _allIngredients;
        private IngredientEntryJson _lastSubstitution;

        public RecipeGeneratorService(NERPredictor ner, MLModel1 intentClassifier)
        {
            _ner = ner;
            _intentClassifier = intentClassifier;
            _nutrition = new NutritionCache();
            _allIngredients = File.ReadAllLines("food.csv")
          .Skip(1)
          .Select(line =>
          {
              var parts = line.Split(',');

              decimal TryParse(string s) => decimal.TryParse(s.Trim('"'), out var val) ? val : 0;

              return new IngredientNutrition
              {
                  Name = parts[1].Trim('"').ToLower(),             // name column
                  Calories = TryParse(parts[11]),                  // Data.Kilocalories
                  Carbs = TryParse(parts[7]),                      // Data.Carbohydrate
                  Protein = TryParse(parts[17]),                   // Data.Protein
                  Fat = TryParse(parts[27]),                       // Data.Fat.Total Lipid
                  Fiber = TryParse(parts[10])                      // Data.Fiber
              };
          })
          .ToList();
        }

        public UpdatedRecipeResultJson GenerateUpdatedRecipe(string userInput, RecipeJson original)
        {
            // Step 1: NLP extraction
            var intent = ClassifyIntent(userInput);
            var entities = _ner.Predict(userInput);

            Console.WriteLine($"🔧 Old: {entities.OldIngredient}, New: {entities.NewIngredient}");

            // Step 2: Clone original recipe
            var updatedRecipe = new RecipeJson
            {
                Name = original.Name + " (Updated)",
                Tag = original.Tag,  // Keep the same tag (e.g., "Vegan", "Vegetarian", etc.)
                Ingredients = original.Ingredients
                    .Select(i => new IngredientEntryJson
                    {
                        Ingredient = i.Ingredient,    // Name of the ingredient (e.g., "Chicken breast")
                        Quantity = i.Quantity,        // Quantity as string (e.g., "100g", "2 shells (60g)")
                        Unit = i.Unit,                // Unit of the ingredient (e.g., "g", "shell", etc.)
                        Calories = i.Calories,        // Calories per 100g or per ingredient
                        Carbs = i.Carbs,              // Carbs per 100g or per ingredient
                        Fat = i.Fat,                  // Fat per 100g or per ingredient
                        Protein = i.Protein           // Protein per 100g or per ingredient
                    })
                    .ToList()
            };

            // Step 3: Apply change
            ApplyIngredientChange(updatedRecipe, intent, entities);

            // Step 4: Rebuild instructions

            // Step 5: Nutrition
            var nutrition = CalculateTotalNutritionJson(updatedRecipe);

            return new UpdatedRecipeResultJson
            {
                Title = updatedRecipe.Name,
                Ingredients = updatedRecipe.Ingredients,
                Calories = nutrition.Calories,
                Protein = nutrition.Protein,
                Carbs = nutrition.Carbs,
                Fat = nutrition.Fat,
                Intent = intent,
                OldIngredient = entities.OldIngredient,
                NewIngredient = entities.NewIngredient
            };
        }

        public string ClassifyIntent(string userInput)
        {
            var input = new IntentClassification.MLModel1.ModelInput { Text = userInput };
            var output = IntentClassification.MLModel1.Predict(input);
            Console.WriteLine($"\n🧠 Intent: {output.PredictedLabel}");
            return output.PredictedLabel;
        }
        public class UpdatedRecipeResult
        {
            public string Title { get; set; }
            public List<IngredientEntry> Ingredients { get; set; }

            public decimal Calories { get; set; }
            public decimal Protein { get; set; }
            public decimal Carbs { get; set; }
            public decimal Fat { get; set; }
            public decimal Fiber { get; set; }

            public string Intent { get; set; }
            public string? OldIngredient { get; set; }
            public string? NewIngredient { get; set; }
        }

        public class UpdatedRecipeResultJson
        {
            public string Title { get; set; }
            public List<IngredientEntryJson> Ingredients { get; set; }

            public decimal Calories { get; set; }
            public decimal Protein { get; set; }
            public decimal Carbs { get; set; }
            public decimal Fat { get; set; }
            public decimal Fiber { get; set; }

            public string Intent { get; set; }
            public string? OldIngredient { get; set; }
            public string? NewIngredient { get; set; }
        }
        private void ApplyIngredientChange(RecipeJson recipe, string intent, IngredientEntities entities)
        {
            if (intent == "substitute" && entities.OldIngredient != null && entities.NewIngredient != null)
            {
                var target = recipe.Ingredients.FirstOrDefault(i =>
                    i.Ingredient.Contains(entities.OldIngredient, StringComparison.OrdinalIgnoreCase));
                if (target != null)
                {
                    // Find nutrition info for the new ingredient
                    var newIngredientNutrition = _allIngredients.FirstOrDefault(i => 
                        i.Name.Contains(entities.NewIngredient, StringComparison.OrdinalIgnoreCase));

                    if (newIngredientNutrition != null)
                    {
                        // Update the ingredient's name and nutrition info
                        target.Ingredient = entities.NewIngredient;
                        target.Calories = newIngredientNutrition.Calories;
                        target.Protein = newIngredientNutrition.Protein;
                        target.Carbs = newIngredientNutrition.Carbs;
                        target.Fat = newIngredientNutrition.Fat;
                    }
                }
            }
            else if (intent == "remove" && entities.OldIngredient != null)
            {
                recipe.Ingredients.RemoveAll(i =>
                    i.Ingredient.Contains(entities.OldIngredient, StringComparison.OrdinalIgnoreCase));
            }
            else if (intent == "add" && entities.NewIngredient != null)
            {
                var newIngredientNutrition = _allIngredients.FirstOrDefault(i => 
                    i.Name.Contains(entities.NewIngredient, StringComparison.OrdinalIgnoreCase));

                if (newIngredientNutrition != null)
                {
                    recipe.Ingredients.Add(new IngredientEntryJson
                    {
                        Ingredient = entities.NewIngredient,
                        Quantity = "100",
                        Unit = "g",
                        Calories = newIngredientNutrition.Calories,
                        Protein = newIngredientNutrition.Protein,
                        Carbs = newIngredientNutrition.Carbs,
                        Fat = newIngredientNutrition.Fat
                    });
                }
            }
        }

        private decimal CalculateTotalCalories(RecipeRec recipe)
        {
            return recipe.Ingredients.Sum(ing =>
            {
                var grams = UnitNormalizer.NormalizeToGrams(ing.Quantity, ing.Unit);
                var nut = _nutrition.GetOrAdd(ing.Name.ToLower(), () =>
                {
                    return _allIngredients.FirstOrDefault(i =>
                        i.Name.Equals(ing.Name, StringComparison.OrdinalIgnoreCase)) ?? new IngredientNutrition();
                });
                return grams / 100 * (nut?.Calories ?? 0);
            });
        }

        private decimal CalculateTotalCaloriesJson(RecipeJson recipe)
        {
            return recipe.Ingredients.Sum(ing =>
            {
                var grams = UnitNormalizer.NormalizeToGramsJson(ing.Quantity.ToString(), ing.Unit);  // Normalize quantity to grams
                return grams / 100 * ing.Calories;
            });
        }
        private (decimal Calories, decimal Protein, decimal Carbs, decimal Fat) CalculateTotalNutritionJson(RecipeJson recipe)
        {
            decimal kcal = 0, protein = 0, carbs = 0, fat = 0;

            foreach (var ing in recipe.Ingredients)
            {
                //var grams = UnitNormalizer.NormalizeToGramsJson(ing.Quantity.ToString(), ing.Unit);
                var grams = NormalizeQuantity(ing.Quantity);
                var factor = grams / 100m;

                kcal += ing.Calories * factor;
                protein += (ing.Protein * factor);
                carbs += (ing.Carbs * factor);
                fat += (ing.Fat * factor);
            }

            return (
                Calories: Math.Round(kcal, 2),
                Protein: Math.Round(protein, 2),
                Carbs: Math.Round(carbs, 2),
                Fat: Math.Round(fat, 2)
            );
        }
        private (decimal Calories, decimal Protein, decimal Carbs, decimal Fat, decimal Fiber)
    CalculateTotalNutrition(RecipeRec recipe)
        {
            decimal kcal = 0, protein = 0, carbs = 0, fat = 0, fiber = 0;

            foreach (var ing in recipe.Ingredients)
            {
                var grams = UnitNormalizer.NormalizeToGrams(ing.Quantity, ing.Unit);
                var nut = _nutrition.GetOrAdd(ing.Name.ToLower(), () =>
                    _allIngredients.Where(i => i.Calories > 1).FirstOrDefault(i =>
                        i.Name.Equals(ing.Name, StringComparison.OrdinalIgnoreCase)) ?? new IngredientNutrition());

                var factor = grams / 100m;
                kcal += (nut.Calories * factor);
                protein += (nut.Protein * factor);
                carbs += (nut.Carbs * factor);
                fat += (nut.Fat * factor);
                fiber += (nut.Fiber * factor);
            }

            return (
                Calories: Math.Round(kcal, 2),
                Protein: Math.Round(protein, 2),
                Carbs: Math.Round(carbs, 2),
                Fat: Math.Round(fat, 2),
                Fiber: Math.Round(fiber, 2)
            );
        }

        public List<RecipeJson> FilterRecipes(List<RecipeJson> allRecipes, decimal maxCalories, string dietaryPreference)
        {
            // Filter recipes based on the max calories and dietary preferences (e.g., Vegan, Vegetarian, Lactose-Free)
            var filteredRecipes = allRecipes.Where(r =>
            {
                // Check the total calories for the recipe
                bool isWithinCalorieLimit = r.TotalCalories <= maxCalories;

                // Check for dietary preferences
                bool matchesDietaryPreference = string.IsNullOrEmpty(dietaryPreference) || r.Tag.Equals(dietaryPreference, StringComparison.OrdinalIgnoreCase);

                return isWithinCalorieLimit && matchesDietaryPreference;
            }).ToList();

            return filteredRecipes;
        }


        public RecipeJson AdjustRecipeToFitCalorieLimit(RecipeJson recipe, decimal maxCalories)
        {
            decimal totalCalories = recipe.TotalCalories;

            // If the recipe exceeds the calorie limit, scale down the ingredients
            if (totalCalories > maxCalories)
            {
                decimal scalingFactor = maxCalories / totalCalories;

                // Adjust the quantity of each ingredient to scale down the recipe's calories
                foreach (var ingredient in recipe.Ingredients)
                {
                    decimal normalizedQuantity = NormalizeQuantity(ingredient.Quantity);

                    // Apply the scaling factor
                    decimal scaledQuantity = normalizedQuantity * scalingFactor;
                }
            }

            return recipe;
        }
        public bool IsCalorieLimitRequest(string userInput, out decimal maxCalories)
        {
            maxCalories = 0;

            // Check for phrases indicating a calorie limit request
            if (userInput.Contains("max", StringComparison.OrdinalIgnoreCase) ||
                userInput.Contains("under", StringComparison.OrdinalIgnoreCase) ||
                userInput.Contains("limit", StringComparison.OrdinalIgnoreCase))
            {
                // Try to find the calorie number (e.g., "400 kcal", "under 400 calories", "max 400 kcal")
                var match = Regex.Match(userInput, @"(\d+)\s*(kcal|calories?)", RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    // Extract the calorie number
                    maxCalories = decimal.Parse(match.Groups[1].Value);
                    return true;
                }
            }

            return false;
        }

        private decimal NormalizeQuantity(string quantity)
        {
            if (string.IsNullOrEmpty(quantity))
                return 0;

            var match = Regex.Match(quantity, @"(\d+)\s*\((\d+)g\)");
            if (match.Success)
            {
                var count = int.Parse(match.Groups[1].Value);
                var totalGrams = int.Parse(match.Groups[2].Value);
                return totalGrams / (decimal)count;
            }

            // fallback: "25g", "100", etc.
            var gramsMatch = Regex.Match(quantity, @"(\d+)\s*g");
            if (gramsMatch.Success)
                return int.Parse(gramsMatch.Groups[1].Value);

            // final fallback: just extract digits
            decimal.TryParse(new string(quantity.Where(char.IsDigit).ToArray()), out var grams);
            return grams;
        }

        public bool TryExtractDietaryPreference(string userInput, out string dietaryPreference)
        {
            dietaryPreference = null;

            // Check for dietary preference keywords
            if (userInput.Contains("vegan", StringComparison.OrdinalIgnoreCase))
            {
                dietaryPreference = "Vegan";
                return true;
            }
            else if (userInput.Contains("vegetarian", StringComparison.OrdinalIgnoreCase))
            {
                dietaryPreference = "Vegetarian";
                return true;
            }
            else if (userInput.Contains("lactose-free", StringComparison.OrdinalIgnoreCase))
            {
                dietaryPreference = "Lactose-Free";
                return true;
            }

            return false;
        }

        public RecipeJson HandleUserRequest(string userInput, List<RecipeJson> allRecipes)
        {
            decimal maxCalories;
            string dietaryPreference = null;

            // Check if user requests a recipe with a max calorie limit
            if (IsCalorieLimitRequest(userInput, out maxCalories))
            {
                Console.WriteLine($"User is looking for a recipe with a maximum of {maxCalories} kcal.");

                // Check if dietary preference is specified
                if (TryExtractDietaryPreference(userInput, out dietaryPreference))
                {
                    Console.WriteLine($"User's dietary preference: {dietaryPreference}");
                }
                else
                {
                    Console.WriteLine("No specific dietary preference specified.");
                }

                // Filter recipes based on maxCalories and dietaryPreference
                var filteredRecipes = FilterRecipes(allRecipes, maxCalories, dietaryPreference);

                // Return the filtered recipe if found
                if (filteredRecipes.Any())
                {
                    Console.WriteLine($"Here is a recipe: {filteredRecipes.First().Name}");
                    return filteredRecipes.First();  // Return the first matching recipe
                }
                else
                {
                    Console.WriteLine("No recipes found matching your criteria.");
                    return null;  // Return null if no matching recipe is found
                }
            }
            else
            {
                Console.WriteLine("User did not specify a calorie limit. Here's a random recipe:");

                // Get a random recipe if no calorie limit is specified
                Random rand = new Random();
                var randomRecipe = allRecipes[rand.Next(allRecipes.Count)];

                // Output the random recipe
                Console.WriteLine($"Here is a random recipe: {randomRecipe.Name}");
                return randomRecipe;  // Return the random recipe
            }
        }
    }


}
