using FireFitBlazor.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeRecommendation
{
    public class RecipeChatService
    {
        private readonly RecipeGeneratorService _recipeService;
        private Dictionary<string, string> _commonQuestions;
        private Dictionary<string, string> _responseTemplates;
        private RecipeJson _currentRecipe;
        public RecipeJson? CurrentRecipe => _currentRecipe;

        public RecipeChatService(RecipeGeneratorService recipeService)
        {
            _recipeService = recipeService;
            InitializeCommonQuestions();
            InitializeResponseTemplates();
        }

        private void InitializeCommonQuestions()
        {
            _commonQuestions = new Dictionary<string, string>
            {
                { "calories", "How many calories are in this recipe?" },
                { "ingredients", "What ingredients do I need?" },
                { "substitutions", "Can I substitute any ingredients?" },
                { "nutrition", "What's the nutritional information?" },
                { "protein", "How much protein is in this recipe?" },
                { "carbs", "How many carbs are in this recipe?" },
                { "fat", "How much fat is in this recipe?" }
            };
        }

        private void InitializeResponseTemplates()
        {
            _responseTemplates = new Dictionary<string, string>
            {
                { "calories", "This recipe contains {0} calories per serving." },
                { "ingredients", "Here are the ingredients you'll need:\n{0}" },
                { "substitutions", "You can substitute:\n{0}" },
                { "nutrition", "Nutritional information per serving:\nCalories: {0}\nProtein: {1}g\nCarbs: {2}g\nFat: {3}g" },
                { "protein", "This recipe contains {0}g of protein per serving." },
                { "carbs", "This recipe contains {0}g of carbohydrates per serving." },
                { "fat", "This recipe contains {0}g of fat per serving." }
            };
        }

        public async Task<string> ProcessMessage(string message, List<RecipeJson> allRecipes)
        {
            // First, try to find a recipe based on preferences
            var (maxCalories, dietaryPreference) = _recipeService.ExtractPreferences(message);
            
            // Filter recipes based on preferences
            var filtered = allRecipes;
            
            // Apply dietary preference filter if specified
            if (!string.IsNullOrEmpty(dietaryPreference))
            {
                filtered = filtered.Where(r => r.Tag.Equals(dietaryPreference, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            if (filtered.Any())
            {
                // Get a random recipe from the filtered list
                var random = new Random();
                var selectedRecipe = filtered[random.Next(filtered.Count)];
                
                // If maxCalories is specified, adjust the recipe portions to fit within the calorie limit
                if (maxCalories.HasValue)
                {
                    decimal currentCalories = (decimal)selectedRecipe.TotalCalories;
                    
                    // If the recipe has more calories than the limit, adjust portions
                    if (currentCalories > maxCalories.Value)
                    {
                        decimal scalingFactor = maxCalories.Value / currentCalories;
                        
                        // Adjust all ingredients proportionally
                        foreach (var ingredient in selectedRecipe.Ingredients)
                        {
                            if (decimal.TryParse(ingredient.Quantity, out decimal quantity))
                            {
                                ingredient.Quantity = (quantity * scalingFactor).ToString("F1");
                            }
                        }

                        // Recalculate nutrition values
                        selectedRecipe.TotalNutrition = new RecipeNutritionalInfo
                        {
                            Calories = (float)Math.Round(currentCalories * scalingFactor, 2),
                            Proteins = (float)Math.Round((decimal)selectedRecipe.TotalNutrition.Proteins  * scalingFactor, 2),
                            Carbs = (float)Math.Round((decimal)selectedRecipe.TotalNutrition.Carbs * scalingFactor, 2),
                            Fats = (float)Math.Round((decimal)selectedRecipe.TotalNutrition.Fats * scalingFactor, 2)
                        };
                    }
                }
                
                // Store the selected recipe for potential modifications
                _currentRecipe = selectedRecipe;
                
                // Format the recipe response
                var response = $"Here's a recipe that matches your criteria:\n\n" +
                             $"🍽️ {selectedRecipe.Name}\n" +
                             $"🔥 Calories: {selectedRecipe.TotalNutrition.Calories} kcal\n" +
                             $"🥩 Protein: {selectedRecipe.TotalNutrition.Proteins}g\n" +
                             $"🍚 Carbs: {selectedRecipe.TotalNutrition.Carbs}g\n" +
                             $"🥑 Fat: {selectedRecipe.TotalNutrition.Fats}g\n\n" +
                             $"Ingredients:\n" +
                             string.Join("\n", selectedRecipe.Ingredients.Select(i => $"• {i.Quantity} {i.Unit} {i.Ingredient}")) +
                             $"\n\nWould you like to make any changes to this recipe? You can add, remove, or substitute ingredients.";
                
                return response;
            }
            else
            {
                return "I couldn't find any recipes matching your criteria. Would you like to try with different preferences?";
            }
        }

        private string GenerateResponse(string questionType)
        {
            if (!_responseTemplates.ContainsKey(questionType))
            {
                return "I'm not sure how to answer that question.";
            }

            var template = _responseTemplates[questionType];
            switch (questionType)
            {
                case "calories":
                    return string.Format(template, _currentRecipe.TotalNutrition.Calories);
                case "ingredients":
                    var ingredients = string.Join("\n", _currentRecipe.Ingredients.Select(i => $"- {i.Quantity} {i.Ingredient}"));
                    return string.Format(template, ingredients);
                case "substitutions":
                    var substitutions = GetAvailableSubstitutions();
                    return string.Format(template, substitutions);
                case "nutrition":
                    return string.Format(template,
                        _currentRecipe.TotalNutrition.Calories,
                        _currentRecipe.TotalNutrition.Proteins,
                        _currentRecipe.TotalNutrition.Carbs,
                        _currentRecipe.TotalNutrition.Fats);
                case "protein":
                    return string.Format(template, _currentRecipe.TotalNutrition.Proteins);
                case "carbs":
                    return string.Format(template, _currentRecipe.TotalNutrition.Carbs);
                case "fat":
                    return string.Format(template, _currentRecipe.TotalNutrition.Fats);
                default:
                    return "I'm not sure how to answer that question.";
            }
        }

        private string GetAvailableSubstitutions()
        {
            var substitutions = new List<string>();
            foreach (var ingredient in _currentRecipe.Ingredients)
            {
                var normalizedName = NormalizeIngredientName(ingredient.Ingredient);
                if (_recipeService.GetIngredientSubstitutions().TryGetValue(normalizedName, out var subs))
                {
                    substitutions.Add($"{ingredient.Ingredient} can be replaced with: {string.Join(", ", subs)}");
                }
            }
            return substitutions.Any() ? string.Join("\n", substitutions) : "No substitutions available for this recipe.";
        }

        private string GenerateGeneralResponse()
        {
            return $"Here's information about {_currentRecipe.Name}:\n" +
                   $"Calories: {_currentRecipe.TotalNutrition.Calories} per serving\n" +
                   $"Protein: {_currentRecipe.TotalNutrition.Proteins}g\n" +
                   $"Carbs: {_currentRecipe.TotalNutrition.Carbs}g\n" +
                   $"Fat: {_currentRecipe.TotalNutrition.Fats}g\n\n" +
                   "You can ask me about specific ingredients, nutritional information, or possible substitutions.";
        }

        private string NormalizeIngredientName(string ingredient)
        {
            return ingredient.ToLower()
                .Replace("fresh", "")
                .Replace("dried", "")
                .Replace("ground", "")
                .Replace("powdered", "")
                .Trim();
        }

        public void SetCurrentRecipe(RecipeJson recipe)
        {
            _currentRecipe = recipe;
        }

        public async Task<string> ProcessRecipeModification(string message)
        {
            if (_currentRecipe == null)
                return "Please first select a recipe before making modifications.";

            var intent = _recipeService.ClassifyIntent(message);
            RecipeRecommendationGen.TrainNERModel();
            var (oldIng, newIng) = RecipeRecommendationGen.ExtractEntitiesFromNer(message);

            if (intent == "substitute")
            {
                if (!string.IsNullOrEmpty(oldIng) && !string.IsNullOrEmpty(newIng))
                {
                    var updatedRecipe = _recipeService.ApplyIngredientChange(_currentRecipe, "substitute",
                        new IngredientEntities { OldIngredient = oldIng, NewIngredient = newIng });

                    _currentRecipe = updatedRecipe;

                    return $"I've substituted {oldIng} with {newIng}. Here's the updated recipe:\n\n" +
                           $"🍽️ {updatedRecipe.Name}\n" +
                           $"🔥 Calories: {updatedRecipe.TotalNutrition.Calories} kcal\n" +
                           $"🥩 Protein: {updatedRecipe.TotalNutrition.Proteins}g\n" +
                           $"🍚 Carbs: {updatedRecipe.TotalNutrition.Carbs}g\n" +
                           $"🥑 Fat: {updatedRecipe.TotalNutrition.Fats}g\n\n" +
                           $"Ingredients:\n" +
                           string.Join("\n", updatedRecipe.Ingredients.Select(i => $"• {i.Quantity} {i.Unit} {i.Ingredient}"));
                }
                return "I couldn't detect both the original and substitute ingredients. Please rephrase.";
            }
            else if (intent == "add")
            {
                if (!string.IsNullOrEmpty(newIng))
                {
                    var updatedRecipe = _recipeService.ApplyIngredientChange(_currentRecipe, "add",
                        new IngredientEntities { NewIngredient = newIng });

                    _currentRecipe = updatedRecipe;

                    return $"I've added {newIng} to the recipe.\n\nUpdated ingredients:\n" +
                           string.Join("\n", updatedRecipe.Ingredients.Select(i => $"• {i.Quantity} {i.Unit} {i.Ingredient}"));
                }
                return "I couldn't detect the ingredient to add. Please rephrase.";
            }
            else if (intent == "remove")
            {
                if (!string.IsNullOrEmpty(oldIng))
                {
                    var updatedRecipe = _recipeService.ApplyIngredientChange(_currentRecipe, "remove",
                        new IngredientEntities { OldIngredient = oldIng });

                    _currentRecipe = updatedRecipe;

                    return $"I've removed {oldIng} from the recipe.\n\nUpdated ingredients:\n" +
                           string.Join("\n", updatedRecipe.Ingredients.Select(i => $"• {i.Quantity} {i.Unit} {i.Ingredient}"));
                }
                return "I couldn't detect the ingredient to remove. Please rephrase.";
            }

            return "I'm not sure what changes you want to make. You can add, remove, or substitute ingredients.";
        
        
        }
        public void ClearCurrentRecipe()
        {
            _currentRecipe = null;
        }

    }
} 