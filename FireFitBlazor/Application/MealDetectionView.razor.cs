using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using Radzen;
using Radzen.Blazor;
using FireFitBlazor.Application.Services;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.ValueObjects;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;
using Microsoft.ML.Data;
using FoodObjectDetection;

using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
namespace FireFitBlazor.Application
{
    public partial class MealDetectionView : ComponentBase
    {
[Parameter] public string? mealType { get; set; }
    private MealType MealType { get; set; }
    [Inject] IHttpClientFactory Http { get; set; }
    private string? capturedImage = null;
    private List<DetectedIngredient> detectedIngredients = new();
    private string MealName = "Nutritional Values";
    private float TotalCalories = 0;
    private float TotalProtein = 0;
    private float TotalCarbs = 0;
    private float TotalFats = 0;
    private string manualEntry;
    private List<Ingredient> allIngredients = new();
    private string mealName = string.Empty;
    private string PrepTime = "11-15 min";

    [Inject]
    public HttpClient HttpClient { get; set; } = default!;
    private string? currentUserId;
    private User? currentUser;


    private IHttpContextAccessor _httpContextAccessor;
    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    private List<(string Label, string Value)> Macros = new();

    private bool isLoading = true;

    private List<CsvFoodRecord> foodDatabase;
    private Dictionary<string, CsvFoodRecord> nameToFoodEntry;

    private async Task OnImageSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        try
        {
            using var stream = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024) // 5 MB
                   .CopyToAsync(stream);

            var base64 = Convert.ToBase64String(stream.ToArray());
            capturedImage = $"data:{file.ContentType};base64,{base64}";
            Console.WriteLine("got here!");
            await LoadDetectedIngredients(capturedImage);
            Console.WriteLine("got here!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    private bool shouldLoad = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && shouldLoad)
        {
            isLoading = true;
            StateHasChanged();
            await Task.Yield();

            capturedImage = ImageTransferService.CapturedImage;
            if (string.IsNullOrEmpty(capturedImage))
            {
                Navigation.NavigateTo("/log-food");
                return;
            }

            detectedIngredients = await LoadDetectedIngredients(capturedImage);

            isLoading = false;
            StateHasChanged();
            await RecalculateMacros();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        var http = HttpClientFactory.CreateClient("ServerAPI");
        var response = await http.GetAsync("/api/customauth/me");
        if (!response.IsSuccessStatusCode)
        {
            Navigation.NavigateTo("/login");
            return;
        }

        currentUser = await response.Content.ReadFromJsonAsync<User>();
        if (currentUser == null)
        {
            Navigation.NavigateTo("/login");
            return;
        }


        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "food.csv");
        var (entries, map) = LoadFoodEntries(csvPath);
        foodDatabase = entries;
        nameToFoodEntry = map;
    

        allIngredients = await FoodLogService.GetAllIngredientNames();
    }

    private async Task<List<DetectedIngredient>> LoadDetectedIngredients(string imageBase64)
    {
        // 1. Save base64 image to temp file
        var tempFilePath = SaveBase64ToTempFileAndResize(imageBase64, 800, 600);

        // 2. Create MLImage from file
        var mlImage = MLImage.CreateFromFile(tempFilePath);

        // 3. Prepare model input
        var input = new ObjectDetection.ModelInput
            {
                Image = mlImage
            };

        // 4. Predict
        var result = ObjectDetection.Predict(input);

        // 5. Parse results
        detectedIngredients = new List<DetectedIngredient>();
        var labels = result.PredictedLabel;
        var boxes = result.PredictedBoundingBoxes;

        bool fallbackToApi = labels == null || labels.Length < 2;
        if (labels == null || labels.Length < 2)
        {
            using var retryStream = new MemoryStream(File.ReadAllBytes(tempFilePath)); // Reload image

            var (apiLabel, apiScore) = await CallFallbackPredictionAPI(retryStream);

            if (!string.IsNullOrWhiteSpace(apiLabel) && apiScore > 0.25f &&
                !detectedIngredients.Any(i => i.Name.Equals(apiLabel, StringComparison.OrdinalIgnoreCase)))
            {
                // Add fallback result as a generic ingredient (position defaults)
                detectedIngredients.Add(new DetectedIngredient(
                    apiLabel,
                    50, 50, 10, 10,
                    "/images/default.png"
                ));
            }
        }

        int boxStride = 4; 
        if (labels != null)
        {



            for (int i = 0; i < labels.Length; i++)
            {
                int boxIndex = i * boxStride;
                float x = boxes[boxIndex];
                float y = boxes[boxIndex + 1];
                float width = boxes[boxIndex + 2];
                float height = boxes[boxIndex + 3];

                float xPercent = x / mlImage.Width * 100f;
                float yPercent = y / mlImage.Height * 100f;
                float widthPercent = width / mlImage.Width * 100f;
                float heightPercent = height / mlImage.Height * 100f;

                detectedIngredients.Add(
         new DetectedIngredient(
             labels[i],              // name
             xPercent,               // x
             yPercent,               // y
             widthPercent,           // width
             heightPercent,          // height
             "/images/default.png"   // iconUrl
         )
         );
            }
        }

        // 6. Clean up temp file
        File.Delete(tempFilePath);

        // 7. Update macros, etc.
        var names = detectedIngredients.Select(i => i.Name).ToList();
        TotalCalories = 0;
        TotalProtein = 0;
        TotalFats = 0;
        TotalCarbs = 0;

        foreach (var ingredient in detectedIngredients)
        {
            ingredient.QuantityGrams = 100; // Default to 100g

            if (nameToFoodEntry.TryGetValue(NormalizeName(ingredient.Name), out var match))
            {
                ingredient.Calories = float.TryParse(match.Calories, out var cal) ? cal : 0;
                ingredient.Protein = float.TryParse(match.Protein, out var pro) ? pro : 0;
                ingredient.Fat = float.TryParse(match.Fat, out var fat) ? fat : 0;
                ingredient.Carbs = float.TryParse(match.Carbs, out var carb) ? carb : 0;

                TotalCalories += (int)(ingredient.Calories);
                TotalProtein += ingredient.Protein;
                TotalFats += ingredient.Fat;
                TotalCarbs += ingredient.Carbs;
            }
        }

        Macros = new()
        {
            ("Calories", $"{TotalCalories}"),
            ("Protein", $"{TotalProtein:F1}g"),
            ("Fats", $"{TotalFats:F1}g"),
            ("Carbs", $"{TotalCarbs:F1}g")
        };
        return detectedIngredients;
    }

    public static string SaveBase64ToTempFileAndResize(string base64, int width, int height)
    {
        var base64Data = base64.Substring(base64.IndexOf(',') + 1);
        byte[] bytes = Convert.FromBase64String(base64Data);
        using var ms = new MemoryStream(bytes);
        using var original = new System.Drawing.Bitmap(ms);
        using var resized = new System.Drawing.Bitmap(original, new System.Drawing.Size(width, height));
        var tempPath = Path.GetTempFileName() + ".png";
        resized.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
        return tempPath;
    }

    // private async Task ValidateMeal()
    // {
    //     var ingredientNames = detectedIngredients.Select(i => i.Name).ToList();
    //     var baseData = await FoodLogService.GetIngredientDetails(ingredientNames);

    //     foreach (var item in detectedIngredients)
    //     {
    //         var match = baseData.FirstOrDefault(x => x.Name == item.Name);
    //         if (match == null) continue;

    //         var ratio = item.QuantityGrams / 100f;

    //         var log = FoodLog.Create(
    //   userId: userId,
    //   foodName: match.Name,
    //   calories: (int)(match.Nutrition.Calories * ratio),
    //   proteins: match.Nutrition.Proteins * ratio,
    //   carbs: match.Nutrition.Carbs * ratio,
    //   fats: match.Nutrition.Fats * ratio
    //   );

    //         await FoodLogService.SaveFoodLogAsync(log);
    //     }

    //     Navigation.NavigateTo("/daily-food-log");
    // }

    public async Task<(string predictedLabel, float score)> CallFallbackPredictionAPI(Stream imageStream)
    {
        using var client = new HttpClient();
        using var content = new MultipartFormDataContent();

        var streamContent = new StreamContent(imageStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        content.Add(streamContent, "file", "image.png");
        var http = Http.CreateClient("MLAPI");
        var response = await http.PostAsync("predict", content);
        if (!response.IsSuccessStatusCode)
        {
            return ("", 0f);
        }

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PredictionResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return (result.PredictedLabel, result.Score);
    }

    public class PredictionResponse
    {
        public string PredictedLabel { get; set; }
        public float Score { get; set; }
    }

    private async Task SaveAsMeal()
    {
        // if (string.IsNullOrWhiteSpace(mealName))
        // {
        //     await JS.InvokeVoidAsync("alert", "Please enter a name for your meal");
        //     return;
        // }

        // if (!detectedIngredients.Any())
        // {
        //     await JS.InvokeVoidAsync("alert", "Please add at least one ingredient to your meal");
        //     return;
        // }

        try
        {

            if (mealType == "breakfast")
                MealType = MealType.Breakfast;
            if (mealType == "lunch")
                MealType = MealType.Lunch;
            if (mealType == "dinner")
                MealType = MealType.Dinner;
          
            var log = FoodLog.Create(
         userId: currentUser.UserId,
         foodName: mealName.Trim(),
         calories: (int)(TotalCalories),
         proteins: TotalProtein,
         carbs: TotalCarbs,
         fats: TotalFats,
         mealType: MealType

     );

            await FoodLogService.SaveFoodLogAsync(log);
            // Clear the form
            mealName = string.Empty;
            detectedIngredients.Clear();

            // Navigate to dashboard
            Navigation.NavigateTo("/dashboard");
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("alert", $"Error saving meal: {ex.Message}");
        }
    }

    private async Task RemoveIngredient(DetectedIngredient ingredient)
    {
        detectedIngredients.Remove(ingredient);
        await RecalculateMacros();
    }

    private async Task AddManualIngredient()
    {
        if (string.IsNullOrWhiteSpace(manualEntry)) return;

        var normalizedName = NormalizeName(manualEntry.Trim());

        if (nameToFoodEntry == null)
        {
            await JS.InvokeVoidAsync("alert", "Ingredient database not loaded. Please try again.");
            return;
        }

        if (!nameToFoodEntry.TryGetValue(normalizedName, out var foodEntry))
        {
            // Try partial match
            foodEntry = foodDatabase.FirstOrDefault(x =>
                NormalizeName(x.Ingredient).Contains(normalizedName) ||
                normalizedName.Contains(NormalizeName(x.Ingredient))
            );
        }

        if (foodEntry != null)
        {
            var ingredient = new DetectedIngredient(
                foodEntry.Ingredient, 
                50,
                50, 
                10, 
                10, 
                "/images/default.png"
            )
                {
                    QuantityGrams = 100, // default quantity
                    Calories = float.Parse(foodEntry.Calories),
                    Protein = float.Parse(foodEntry.Protein),
                    Fat = float.Parse(foodEntry.Fat),
                    Carbs = float.Parse(foodEntry.Carbs)
                };

            detectedIngredients.Add(ingredient);
            manualEntry = string.Empty;
            await RecalculateMacros();
        }
        else
        {
            await JS.InvokeVoidAsync("alert", "Ingredient not found in database. Please try a different name.");
        }
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "";

        return name
            .ToLowerInvariant()
            .Replace(",", " ")
            .Replace("&", " and ")
            .Replace("(", " ")
            .Replace(")", " ")
            .Replace("'", "")
            .Replace(".", " ")
            .Replace("/", " ")
            .Replace("-", " ")
            .Trim()
            .Replace("  ", " "); // Collapse multiple spaces
    }

    private async Task RecalculateMacros()
    {
        TotalCalories = 0;
        TotalProtein = 0;
        TotalFats = 0;
        TotalCarbs = 0;

        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "food.csv");
        var (foodData, nameToEntry) = LoadFoodEntries(csvPath);

        foreach (var ingredient in detectedIngredients)
        {
            var normalized = NormalizeName(ingredient.Name);
            CsvFoodRecord match = null;

            // Try exact match first
            if (!nameToEntry.TryGetValue(normalized, out match))
            {
                // Fallback: try partial match
                match = nameToEntry
                    .FirstOrDefault(kvp => NormalizeName(kvp.Key).Contains(normalized))
                    .Value;
            }

            if (match != null)
            {
                // float ratio = ingredient.QuantityGrams / 100f;

                // ingredient.Calories = match.Calories;
                // ingredient.Protein = match.Protein;
                // ingredient.Fat = match.Fat;
                // ingredient.Carbs = match.Carbs;

                // TotalCalories += (int)(match.Calories * ratio);
                // TotalProtein += match.Protein * ratio;
                // TotalFats += match.Fat * ratio;
                // TotalCarbs += match.Carbs * ratio;

                float ratio = ingredient.QuantityGrams / 100f;

                ingredient.Calories = float.TryParse(match.Calories, out var cal) ? cal : 0;
                ingredient.Protein = float.TryParse(match.Protein, out var pro) ? pro : 0;
                ingredient.Fat = float.TryParse(match.Fat, out var fat) ? fat : 0;
                ingredient.Carbs = float.TryParse(match.Carbs, out var carb) ? carb : 0;

                TotalCalories += (int)(ingredient.Calories * ratio);
                TotalProtein += ingredient.Protein * ratio;
                TotalFats += ingredient.Fat * ratio;
                TotalCarbs += ingredient.Carbs * ratio;
            }
        }

        Macros = new()
    {
        ("Calories", $"{TotalCalories}"),
        ("Protein", $"{TotalProtein:F1}g"),
        ("Fats", $"{TotalFats:F1}g"),
        ("Carbs", $"{TotalCarbs:F1}g"),
    };
    }


    // public static (List<FoodEntry> entries, Dictionary<string, FoodEntry> nameToEntry) LoadFoodEntries(string path)
    // {
    //     using var reader = new StreamReader(path);
    //     using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    //     var records = csv.GetRecords<CsvFoodRecord>().ToList();

    //     var entries = new List<FoodEntry>();
    //     var nameToEntry = new Dictionary<string, FoodEntry>();

    //     foreach (var record in records)
    //     {
    //         string originalName = record.Ingredient;
    //         //string normalized = NormalizeName(originalName);

    //         bool parsedCalories = float.TryParse(record.Calories, out float calories);
    //         bool parsedProtein = float.TryParse(record.Protein, out float protein);
    //         bool parsedFat = float.TryParse(record.Fat, out float fat);
    //         bool parsedCarbs = float.TryParse(record.Carbs, out float carbs);

    //         if (parsedCalories && parsedProtein && parsedFat && parsedCarbs)
    //         {
    //             var entry = new FoodEntry
    //                 {
    //                     Name = originalName,
    //                     Calories = calories,
    //                     Protein = protein,
    //                     Fat = fat,
    //                     Carbs = carbs
    //                 };

    //             entries.Add(entry);
    //             nameToEntry[originalName] = entry;
    //         }
    //     }

    //     return (entries, nameToEntry);
    // }


    public static (List<CsvFoodRecord>, Dictionary<string, CsvFoodRecord>) LoadFoodEntries(string path)
{
    using var reader = new StreamReader(path);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    var records = csv.GetRecords<CsvFoodRecord>().ToList();
    var nameToEntry = new Dictionary<string, CsvFoodRecord>();

    foreach (var record in records)
    {
        if (!string.IsNullOrWhiteSpace(record.Ingredient))
        {
            nameToEntry[record.Ingredient] = record;
        }
    }

    return (records, nameToEntry);
}

    public class CsvFoodRecord
    {
        [Name("Ingredient")]
        public string Ingredient { get; set; }

        [Name("Calories")]
        public string Calories { get; set; }

        [Name("Protein")]
        public string Protein { get; set; }

        [Name("Carbohydrates")]
        public string Carbs { get; set; }

        [Name("Fat")]
        public string Fat { get; set; }
    }

    public class FoodEntry
    {
        public string Name { get; set; }
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Fat { get; set; }
        public float Carbs { get; set; }
    }

    private void AdjustQuantity(DetectedIngredient ingredient, int adjustment)
    {
        ingredient.QuantityGrams = Math.Max(0, ingredient.QuantityGrams + adjustment);
        RecalculateMacros();
    }
    }
}

