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
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components.Web;
using FireFitBlazor.Components;
using FireFitBlazor.Domain.ContextInterfaces;
using Radzen;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Application.Services;
using FireFitBlazor.Domain.ValueObjects;
using FireFitBlazor.Domain.Enums;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Application
{
    public partial class LogFood : ComponentBase
    {
[Parameter] public string? mealType { get; set; }
    private MealType MealType { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private User? currentUser;

    private string searchTerm = "";
    private bool isLoading = false;
    private List<Ingredient> allIngredients = new();
    private List<Ingredient> FilteredIngredients =>
     string.IsNullOrWhiteSpace(searchTerm)
         ? historyIngredients
         : allIngredients
             .Where(i =>
                 i.Name.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                 i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
             .ToList();

    private List<Ingredient> historyIngredients = new();

    private MealType ParsedMealType => Enum.TryParse<MealType>(mealType, true, out var result)
    ? result
    : MealType.Breakfast;

    private string? selectedMeal;

    private List<MealModel> meals = new()
    {
        new MealModel { Name = "Breakfast", Icon = "??" },
        new MealModel { Name = "Lunch", Icon = "??" },
        new MealModel { Name = "Dinner", Icon = "??" },
    };

    public class MealModel
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
    }

    private bool showBarcodeScanner = false;

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

        if (!string.IsNullOrWhiteSpace(mealType))
        {
            var normalized = char.ToUpper(mealType[0]) + mealType[1..].ToLower();
            if (meals.Any(m => m.Name == normalized))
            {
                selectedMeal = normalized;
            }
        }

        allIngredients = await FoodLogService.GetAllIngredientNames();
        await LoadUserHistory();
    }

    private void OpenBarcodeScanner()
    {
        if (string.IsNullOrWhiteSpace(mealType) && string.IsNullOrWhiteSpace(selectedMeal))
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Meal type required", "Please select a meal type before scanning a barcode.");
            return;
        }
        showBarcodeScanner = true;
    }

    private async Task OnImageSelected(InputFileChangeEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(mealType) && string.IsNullOrWhiteSpace(selectedMeal))
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Meal type required", "Please select a meal type before scanning a meal.");
            return;
        }

        var file = e.File;
        try
        {
            isLoading = true;
            StateHasChanged();

            using var stream = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024).CopyToAsync(stream);
            var base64 = Convert.ToBase64String(stream.ToArray());

            ImageTransferService.CapturedImage = $"data:{file.ContentType};base64,{base64}";
            await Task.Delay(50);
            Navigation.NavigateTo($"/meal-detect-demo?mealType={mealType ?? selectedMeal?.ToLower()}");
        }
        catch
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task ShowQuickAddDialog()
    {
        if (string.IsNullOrWhiteSpace(mealType) && string.IsNullOrWhiteSpace(selectedMeal))
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Meal type required", "Please select a meal type before adding a food.");
            return;
        }

        try
        {
            var result = await DialogService.OpenAsync<QuickAddDialog>(
                "Quick Add Food",
                new Dictionary<string, object>
                        {
                { "UserId", currentUser.UserId },
                { "MealType", mealType ?? selectedMeal?.ToLower() }
                        },
                new DialogOptions { Width = "500px", Height = "auto", Resizable = true, Draggable = true }  );

            if (result is bool shouldReload && shouldReload)
            {
                await LoadUserHistory();
            }
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Error", "Failed to open Quick Add dialog: " + ex.Message);
        }
    }

    private async Task LoadUserHistory()
    {
        if (currentUser != null)
        {
            var history = await FoodLogService.GetUserIngredientHistoryAsync(currentUser.UserId);
            var ingredientNames = history.Select(h => h.IngredientId).ToList();

            historyIngredients = await FoodLogService.GetIngredientDetailsById(
                ingredientNames.Select(id => id).ToList()
            );
        }
    }

    private async Task AddIngredient(string name)
    {
        if (string.IsNullOrWhiteSpace(mealType) && string.IsNullOrWhiteSpace(selectedMeal))
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Meal type required", "Please select a meal type before adding an ingredient.");
            return;
        }

        var ingredient = allIngredients.FirstOrDefault(i => i.Name == name);
        if (ingredient != null)
        {
            await FoodLogService.AddToUserHistoryAsync(currentUser.UserId, ingredient.IngredientId, ingredient.Name);
            await LoadUserHistory();
        }

        var meal = !string.IsNullOrWhiteSpace(mealType) ? mealType : selectedMeal?.ToLower();
        Navigation.NavigateTo($"/ingredient-details/{ingredient.IngredientId}/{meal}");
    }

    private string Capitalize(string value)
    {
        return string.IsNullOrEmpty(value) ? value : char.ToUpper(value[0]) + value[1..];
    }

    private void OnMealChanged(object value)
    {
        var newMeal = value?.ToString()?.ToLowerInvariant();
        if (!string.IsNullOrEmpty(newMeal))
        {
            Navigation.NavigateTo($"/log-food/{newMeal}", forceLoad: false);
        }
    }

    private void HandleInput(ChangeEventArgs e)
    {
        searchTerm = e.Value?.ToString() ?? "";
    }

    private void CloseBarcodeScanner()
    {
        showBarcodeScanner = false;
    }
    }
}


