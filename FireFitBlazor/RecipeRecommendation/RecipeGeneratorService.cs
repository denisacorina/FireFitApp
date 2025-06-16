using IntentClassification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BioTaggedSentence;
using static Tensorflow.TensorSliceProto.Types;
using static TorchSharp.torch.nn;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        private void ConvertTxtToJson(string txtPath, string jsonPath)
        {
            try
            {
                Console.WriteLine($"Converting {txtPath} to JSON format...");
                
                var examples = File.ReadAllLines(txtPath)
                    .Skip(1) // Skip header
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line =>
                    {
                        var parts = line.Split('\t');
                        if (parts.Length != 2)
                        {
                            Console.WriteLine($"Warning: Skipping malformed line: {line}");
                            return null;
                        }

                        return new BioTagGenerator.NerExample
                        {
                            Intent = parts[0].Trim(),
                            Text = parts[1].Trim()
                        };
                    })
                    .Where(x => x != null)
                    .ToList();

                var json = JsonSerializer.Serialize(examples, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                File.WriteAllText(jsonPath, json);
                Console.WriteLine($"✅ Converted {examples.Count} examples to {jsonPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting TXT to JSON: {ex.Message}");
                throw;
            }
        }

        public void RegenerateBioTags()
        {
            try
            {
                Console.WriteLine("Starting BIO tag generation...");

                //ConvertTxtToJson(
                //    "nlp_recipe_intents_15000.txt",
                //    "nlp_recipe_intents_15000.json"
                //);


                //BioTagGenerator.GenerateBIO(
                //"nlp_recipe_intents_15000.json",
                //    "food.csv",
                //    "bio_annotated_dataset_old_new3.json"
                //);

                Console.WriteLine("BIO tag generation completed successfully!");

                // Train the model with new dataset
                //var flat = RecipeRecommendationGen.FlattenBioJson("bio_annotated_dataset_old_new3.json");
                //var contextData = RecipeRecommendationGen.ConvertToTokenInputWithContext(flat);
                //using (var writer = new StreamWriter("bio_dataset_flat3.tsv"))
                //{
                //    writer.WriteLine("PrevToken\tToken\tNextToken\tLabel");
                //    foreach (var row in contextData)
                //    {
                //        writer.WriteLine($"{row.PrevToken}\t{row.Token}\t{row.NextToken}\t{row.Label}");
                //    }
                //}

                //RecipeRecommendationGen.TrainML();
                //RecipeRecommendationGen.PredictFromSavedModel("replace coconut milk with almond milk");
                //RecipeRecommendationGen.PredictFromSavedModel("add honey");
                //RecipeRecommendationGen.PredictFromSavedModel("add onion");
                //RecipeRecommendationGen.PredictFromSavedModel("exclude yogurt");
                //RecipeRecommendationGen.PredictFromSavedModel("remove onion");
                //RecipeRecommendationGen.PredictFromSavedModel("swap quinoa for rice");
                //RecipeRecommendationGen.PredictFromSavedModel("please add vegan cheese");
                //RecipeRecommendationGen.PredictFromSavedModel("use oat flour instead of almond flour");
                //RecipeRecommendationGen.PredictFromSavedModel("skip the butter");
                //RecipeRecommendationGen.PredictFromSavedModel("get rid of eggs");
                //RecipeRecommendationGen.PredictFromSavedModel("include honey and sugar");
                //RecipeRecommendationGen.PredictFromSavedModel("eliminate beef");
                //RecipeRecommendationGen.PredictFromSavedModel("replace beef with tofu");
                //RecipeRecommendationGen.PredictFromSavedModel("no coconut milk in this");
                //RecipeRecommendationGen.PredictFromSavedModel("remove the olive oil from the list");
                //RecipeRecommendationGen.PredictFromSavedModel("add coconut oil to my recipe");

                //RecipeRecommendationGen.TestPrediction();

                //// Train and test intent classification
                ////RecipeRecommendationGen.TrainModelForTextClassification();
                ////RecipeRecommendationGen.PredictUserIntent();

                Console.WriteLine("Model training and testing completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during BIO tag generation and model training: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
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
        public class IngredientMatch
        {
            public IngredientNutrition Ingredient { get; set; }
            public double Confidence { get; set; }
            public string MatchedName { get; set; }
        }

        private IngredientMatch FindBestIngredientMatch(string ingredientName)
        {
            var matches = new List<IngredientMatch>();
            
            // Normalize the input ingredient name
            var normalizedInput = ingredientName.ToLowerInvariant().Trim();
            
            foreach (var ing in _allIngredients)
            {
                var normalizedIng = ing.Name.ToLowerInvariant().Trim();
                double confidence = 0.0;
                
                // Exact match
                if (normalizedInput == normalizedIng)
                {
                    confidence = 1.0;
                }
                // Contains match
                else if (normalizedInput.Contains(normalizedIng) || normalizedIng.Contains(normalizedInput))
                {
                    // Calculate confidence based on length ratio
                    var shorter = Math.Min(normalizedInput.Length, normalizedIng.Length);
                    var longer = Math.Max(normalizedInput.Length, normalizedIng.Length);
                    confidence = (double)shorter / longer;
                }
                
                if (confidence > 0.5) // Only consider matches with confidence > 50%
                {
                    matches.Add(new IngredientMatch 
                    { 
                        Ingredient = ing, 
                        Confidence = confidence,
                        MatchedName = ing.Name
                    });
                }
            }
            
            return matches.OrderByDescending(m => m.Confidence).FirstOrDefault();
        }

        private bool ValidateNutritionValues(IngredientEntryJson ingredient)
        {
            // Define reasonable ranges for nutrition values per 100g
            const decimal MAX_CALORIES = 900; // Highest caloric foods like oils
            const decimal MAX_PROTEIN = 40;   // High protein foods
            const decimal MAX_CARBS = 100;    // High carb foods
            const decimal MAX_FAT = 100;      // High fat foods
            
            const decimal MIN_CALORIES = 0;
            const decimal MIN_PROTEIN = 0;
            const decimal MIN_CARBS = 0;
            const decimal MIN_FAT = 0;
            
            // Check if values are within reasonable ranges
            bool isValid = true;
            
            if (ingredient.Calories < MIN_CALORIES || ingredient.Calories > MAX_CALORIES)
            {
                Console.WriteLine($"Warning: Unusual calorie value for {ingredient.Ingredient}: {ingredient.Calories}");
                isValid = false;
            }
            
            if (ingredient.Protein < MIN_PROTEIN || ingredient.Protein > MAX_PROTEIN)
            {
                Console.WriteLine($"Warning: Unusual protein value for {ingredient.Ingredient}: {ingredient.Protein}");
                isValid = false;
            }
            
            if (ingredient.Carbs < MIN_CARBS || ingredient.Carbs > MAX_CARBS)
            {
                Console.WriteLine($"Warning: Unusual carb value for {ingredient.Ingredient}: {ingredient.Carbs}");
                isValid = false;
            }
            
            if (ingredient.Fat < MIN_FAT || ingredient.Fat > MAX_FAT)
            {
                Console.WriteLine($"Warning: Unusual fat value for {ingredient.Ingredient}: {ingredient.Fat}");
                isValid = false;
            }
            
            // Check if total macronutrients make sense (should be close to 100g)
            var totalMacros = ingredient.Protein + ingredient.Carbs + ingredient.Fat;
            if (totalMacros > 105 || totalMacros < 95)
            {
                Console.WriteLine($"Warning: Total macros for {ingredient.Ingredient} don't add up to 100g: {totalMacros}g");
                isValid = false;
            }
            
            return isValid;
        }

        private void ApplyIngredientChange(RecipeJson recipe, string intent, IngredientEntities entities)
        {
            if (intent == "substitute" && entities.OldIngredient != null && entities.NewIngredient != null)
            {
                // Find the target ingredient using fuzzy matching
                var target = recipe.Ingredients.FirstOrDefault(i =>
                    i.Ingredient.Contains(entities.OldIngredient, StringComparison.OrdinalIgnoreCase) ||
                    entities.OldIngredient.Contains(i.Ingredient, StringComparison.OrdinalIgnoreCase));

                if (target != null)
                {
                    // Find nutrition info for the new ingredient using confidence scoring
                    var newIngredientMatch = FindBestIngredientMatch(entities.NewIngredient);

                    if (newIngredientMatch != null && newIngredientMatch.Confidence >= 0.7) // Require 70% confidence
                    {
                        // Preserve the original quantity and unit
                        var originalQuantity = target.Quantity;
                        var originalUnit = target.Unit;

                        // Update the ingredient's name and nutrition info
                        target.Ingredient = newIngredientMatch.MatchedName;
                        target.Calories = newIngredientMatch.Ingredient.Calories;
                        target.Protein = newIngredientMatch.Ingredient.Protein;
                        target.Carbs = newIngredientMatch.Ingredient.Carbs;
                        target.Fat = newIngredientMatch.Ingredient.Fat;

                        // Keep the original quantity and unit
                        target.Quantity = originalQuantity;
                        target.Unit = originalUnit;

                        // Validate the nutrition values
                        if (!ValidateNutritionValues(target))
                        {
                            Console.WriteLine($"Warning: Nutrition values for {target.Ingredient} may be incorrect");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Could not find a good match for {entities.NewIngredient} (confidence: {newIngredientMatch?.Confidence ?? 0})");
                    }
                }
            }
            else if (intent == "remove" && entities.OldIngredient != null)
            {
                // Use fuzzy matching for removal
                var ingredientsToRemove = recipe.Ingredients
                    .Where(i => i.Ingredient.Contains(entities.OldIngredient, StringComparison.OrdinalIgnoreCase) ||
                               entities.OldIngredient.Contains(i.Ingredient, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (ingredientsToRemove.Any())
                {
                    foreach (var ingredient in ingredientsToRemove)
                    {
                        recipe.Ingredients.Remove(ingredient);
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: Could not find ingredient to remove: {entities.OldIngredient}");
                }
            }
            else if (intent == "add" && entities.NewIngredient != null)
            {
                // Check if ingredient already exists
                var existingIngredient = recipe.Ingredients.FirstOrDefault(i =>
                    i.Ingredient.Equals(entities.NewIngredient, StringComparison.OrdinalIgnoreCase));

                if (existingIngredient == null)
                {
                    // Find nutrition info using confidence scoring
                    var newIngredientMatch = FindBestIngredientMatch(entities.NewIngredient);

                    if (newIngredientMatch != null && newIngredientMatch.Confidence >= 0.7) // Require 70% confidence
                    {
                        var newIngredient = new IngredientEntryJson
                        {
                            Ingredient = newIngredientMatch.MatchedName,
                            Quantity = "100", // Default quantity
                            Unit = "g",      // Default unit
                            Calories = newIngredientMatch.Ingredient.Calories,
                            Protein = newIngredientMatch.Ingredient.Protein,
                            Carbs = newIngredientMatch.Ingredient.Carbs,
                            Fat = newIngredientMatch.Ingredient.Fat
                        };

                        // Validate the nutrition values
                        if (ValidateNutritionValues(newIngredient))
                        {
                            recipe.Ingredients.Add(newIngredient);
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Nutrition values for {newIngredient.Ingredient} may be incorrect");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Could not find a good match for {entities.NewIngredient} (confidence: {newIngredientMatch?.Confidence ?? 0})");
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: Ingredient {entities.NewIngredient} already exists in the recipe");
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
                    Random rand = new Random();
                    var randomFilteredRecipe = filteredRecipes[rand.Next(filteredRecipes.Count)];
                    return randomFilteredRecipe; 
                }
                else
                {
                    Console.WriteLine("No recipes found matching criteria.");
                    return null;  
                }
            }
            else
            {
                Console.WriteLine("User did not specify a calorie limit.");

                Random rand = new Random();
                var randomRecipe = allRecipes[rand.Next(allRecipes.Count)];

               
                Console.WriteLine($"Here is a random recipe: {randomRecipe.Name}");
                return randomRecipe;  
            }
        }
    }


}
