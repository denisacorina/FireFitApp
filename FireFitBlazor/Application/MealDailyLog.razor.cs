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
using FireFitBlazor.Components.Dialogs;
using FireFitBlazor.Domain.ValueObjects;
using FireFitBlazor.Domain.Enums;

namespace FireFitBlazor.Application
{
    public partial class MealDailyLog : ComponentBase
    {
private DateTime selectedDate = DateTime.Today;
    private int caloriesConsumed = 0;
    private int totalCaloriesGoal = 1750;
    private int exerciseCalories = 0;
    private int RemainingCalories => totalCaloriesGoal - caloriesConsumed + exerciseCalories;
    private int proteinGoal = 0;
    private int carbsGoal = 0;
    private int fatsGoal = 0;
    private List<MealModel> meals = new()
    {
        new MealModel { Name = "Breakfast", Icon = "??" },
        new MealModel { Name = "Lunch", Icon = "??" },
        new MealModel { Name = "Dinner", Icon = "??" },
    };

    private List<FoodLog> todaysFoodLogs = new();

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    private User? currentUser;
    private Guid? swipedItemId = null;

    protected override async Task OnInitializedAsync()
    {
        var http = HttpClientFactory.CreateClient("ServerAPI");
        var response = await http.GetAsync("/api/customauth/me");
        if (!response.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }

        currentUser = await response.Content.ReadFromJsonAsync<User>();
        if (currentUser == null)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }

        await LoadTodaysFoodLogs();
    }

    private async Task LoadTodaysFoodLogs()
    {
        todaysFoodLogs = await FoodLogService.GetLogsForDate(currentUser.UserId, selectedDate);
        caloriesConsumed = (int)todaysFoodLogs.Sum(f => f.NutritionalInfo.Calories);

        totalCaloriesGoal = await FoodLogService.GetDailyGoalCalories(currentUser.UserId);

        var macroGoals = await GoalService.GetUserMacroGoalsAsync(currentUser.UserId);
        proteinGoal = (int)macroGoals.Proteins;
        carbsGoal = (int)macroGoals.Carbs;
        fatsGoal = (int)macroGoals.Fats;
    }

    private Task ChangeDate(int offset)
    {
        selectedDate = selectedDate.AddDays(offset);
        return LoadTodaysFoodLogs();
    }

    private void NavigateToAddFood(string meal) => NavigationManager.NavigateTo($"/log-food/{meal.ToLowerInvariant()}", true);

    private async Task EditFood(FoodLog log)
    {
        var result = await DialogService.OpenAsync<EditFoodLogDialog>(
            title: "Edit Food Log",
            parameters: new Dictionary<string, object> { ["EditableLog"] = log },
            options: new DialogOptions { Width = "600px", Resizable = true, Draggable = true }
        );

        if (result is bool shouldReload && shouldReload)
        {
            await LoadTodaysFoodLogs();
        }
    }

    private async Task DeleteFood(Guid foodId)
    {
        await FoodLogService.DeleteLog(foodId);
        await LoadTodaysFoodLogs();
    }

    public class MealModel
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
    }

    private void HandleSwipeToggle(Guid id)
    {
        if (swipedItemId == id)
        {
            swipedItemId = null;
        }
        else
        {
            swipedItemId = id;
        }
    }

    private async Task ShowFinishDayDialog()
    {
        try
        {
            if (caloriesConsumed >= 1000)
            {

                var parameters = new Dictionary<string, object>
                {
                { "FoodLogs", todaysFoodLogs },
                { "CalorieGoal", totalCaloriesGoal },
                { "ExerciseCalories", exerciseCalories },
                { "ProteinGoal", proteinGoal },
                { "CarbsGoal", carbsGoal },
                { "FatsGoal", fatsGoal }
                };

                var options = new DialogOptions
                {
                        Width = "500px",
                        Height = "auto",
                        Resizable = true,
                        Draggable = true,
                        CloseDialogOnEsc = true,
                        CloseDialogOnOverlayClick = true,
                        ShowClose = true
                };

                await DialogService.OpenAsync<FinishDaySummaryDialog>("Daily Summary", parameters, options);
            }

            NotificationService.Notify(NotificationSeverity.Info, "Not enough calories logged", "You must have at least 1000 calories consumed daily.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ShowFinishDayDialog: {ex}");
            NotificationService.Notify(NotificationSeverity.Error, "Error", "Failed to open daily summary dialog: " + ex.Message);
        }
    }

    private bool IsCaloriesInRange()
    {
        var percentage = (double)caloriesConsumed / totalCaloriesGoal;
        return percentage >= 0.8 && percentage <= 1.2;
    }

    private string GetCalorieMessage()
    {
        var percentage = (double)caloriesConsumed / totalCaloriesGoal;
        if (percentage < 0.8)
            return "You're under your calorie goal";
        else
            return "You're over your calorie goal";
    }

    private bool IsProteinInRange()
    {
        var totalProtein = todaysFoodLogs.Sum(f => f.NutritionalInfo.Proteins);
        var percentage = totalProtein / proteinGoal;
        return percentage >= 0.8 && percentage <= 1.2;
    }

    private string GetProteinMessage()
    {
        var totalProtein = todaysFoodLogs.Sum(f => f.NutritionalInfo.Proteins);
        var percentage = totalProtein / proteinGoal;
        if (percentage < 0.8)
            return "You're under your protein goal";
        else
            return "You're over your protein goal";
    }

    private bool IsCarbsInRange()
    {
        var totalCarbs = todaysFoodLogs.Sum(f => f.NutritionalInfo.Carbs);
        var percentage = totalCarbs / carbsGoal;
        return percentage >= 0.8 && percentage <= 1.2;
    }

    private string GetCarbsMessage()
    {
        var totalCarbs = todaysFoodLogs.Sum(f => f.NutritionalInfo.Carbs);
        var percentage = totalCarbs / carbsGoal;
        if (percentage < 0.8)
            return "You're under your carbs goal";
        else
            return "You're over your carbs goal";
    }

    private bool IsFatsInRange()
    {
        var totalFats = todaysFoodLogs.Sum(f => f.NutritionalInfo.Fats);
        var percentage = totalFats / fatsGoal;
        return percentage >= 0.8 && percentage <= 1.2;
    }

    private string GetFatsMessage()
    {
        var totalFats = todaysFoodLogs.Sum(f => f.NutritionalInfo.Fats);
        var percentage = totalFats / fatsGoal;
        if (percentage < 0.8)
            return "You're under your fats goal";
        else
            return "You're over your fats goal";
    }

    private bool HasEnoughMeals()
    {
        return GetMealCount() >= 2; // At least 2 meals logged
    }

    private int GetMealCount()
    {
        return todaysFoodLogs.Select(f => f.MealType).Distinct().Count();
    }
    }
}
