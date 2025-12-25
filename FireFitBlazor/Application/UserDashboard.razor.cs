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
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Application.Services;
using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.ValueObjects;
using FireFitBlazor.Domain.Enums;
using FireFitBlazor.Components;
using Radzen;
using BlazorBootstrap;

namespace FireFitBlazor.Application
{
    public partial class UserDashboard : ComponentBase
    {
private int totalCaloriesGoal = 0;
    private int caloriesConsumed = 0;
    private int exerciseCalories = 0;
    private List<WeightLog> measurementWeightLogs = new();
    WeightLog? startingWeightLog = null;
    WeightLog? currentWeightLog = null;

    List<WeightLog> totalWeightLogs = new();

    private int RemainingCalories => totalCaloriesGoal - caloriesConsumed + exerciseCalories;
    private int CalculateProgress => (int)(((double)(totalCaloriesGoal - RemainingCalories) / totalCaloriesGoal) * 100);

    private LineChart? lineChart;
    private LineChartOptions lineChartOptions = default!;
    private LineChartOptions? chartOptions;
    private ChartData chartData = default!;
    private List<FoodLog> todaysFoodLogs = new();
    private List<WeightLog> allWeightLogs = new();
    private User? currentUser;
    private List<WorkoutPreference> _workoutSchedule = new();
    private WorkoutPreference? _nextWorkoutPreference;

    protected IEnumerable<WorkoutPreference> WorkoutSchedule => _workoutSchedule;
    protected bool HasWorkoutSchedule => _workoutSchedule.Count > 0;
    protected string NextWorkoutSummary => _nextWorkoutPreference is null
        ? "No workouts scheduled yet. Add them from your profile."
        : $"Next: {_nextWorkoutPreference.Type} on {GetNextWorkoutOccurrence(_nextWorkoutPreference!).ToString("ddd, MMM d 'at' HH:mm")}";

    private bool _weightCheckDialogShown = false;

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    private bool isChartReady = false;
    private bool _initialized = false;

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

        UpdateWorkoutSchedule();

        await LoadTodaysCalories();
        await LoadUserWeightProgress();

        var weights = allWeightLogs
           .Select(w => (double)w.Weight)
           .ToList();

        var dates = allWeightLogs
            .Select(w => w.Date.ToString("dd MMM"))
            .ToList();

        await LoadDashboardData();
        StateHasChanged();
    }


    private async Task LoadDashboardData()
    {
        await LoadTodaysCalories();
        await LoadUserWeightProgress();

        var cleanedLogs = allWeightLogs
            .Where(w => w.Weight > 0 && !double.IsNaN((double)w.Weight))
            .GroupBy(w => w.Date)
            .Select(g => g.OrderByDescending(x => x.Date).First())
            .OrderBy(w => w.Date)
            .ToList();

        var weights = cleanedLogs.Select(w => (double)w.Weight).ToList();
        var dates = cleanedLogs.Select(w => w.Date.ToString("dd MMM")).ToList();

        chartData = new ChartData
            {
                Labels = dates,
                Datasets = new List<IChartDataset>
        {
            new LineChartDataset
            {
                Label = "Weight Change",
                Data = weights,
                BorderColor = new List<string> {"rgba(75,192,192,1)" },
                BackgroundColor = new List<string> {"rgba(75,192,192,0.2)" },
                BorderWidth = new List<double> { 2 },
                Fill = false,
                PointRadius = new List<int> { 5 },
                PointHoverRadius = new List<int> { 6 }
            }
        }
            };

        lineChartOptions = new()
            {
                Responsive = true,
                Interaction = new Interaction { Mode = InteractionMode.Index }
            };

        lineChartOptions.Scales.X = new()
            {
                Title = new() { Text = "Date", Display = true }
            };

        lineChartOptions.Scales.Y = new()
            {
                Title = new() { Text = "Weight (kg)", Display = true }
            };

        lineChartOptions.Plugins.Title = new()
            {
                Display = true,
                Text = "Weight Change Over Time"
            };
    }



    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await lineChart.InitializeAsync(chartData, lineChartOptions);

        var lastWeightLog = allWeightLogs.OrderByDescending(w => w.Date).FirstOrDefault();
        if (lastWeightLog != null && lastWeightLog.Date < DateOnly.FromDateTime(DateTime.Today) && !_weightCheckDialogShown)
        {
            _weightCheckDialogShown = true;
            await ShowWeightCheckDialog();
        }
    }

    private async Task LoadTodaysCalories()
    {
        totalCaloriesGoal = await FoodLogService.GetDailyGoalCalories(currentUser.UserId);

        todaysFoodLogs = await FoodLogService.GetLogsForDate(currentUser.UserId, DateTime.Today);
        caloriesConsumed = todaysFoodLogs.Sum(f => (int)f.NutritionalInfo.Calories);
    }

    private async Task LoadUserWeightProgress()
    {
        var userProgress = await UserProgressContext.GetUserProgressAsync(currentUser.UserId);
        if (userProgress.IsSuccess)
        {
            startingWeightLog = new WeightLog
                {
                    Weight = userProgress.Value.StartingWeight,
                    Date = DateOnly.FromDateTime(userProgress.Value.CreatedAt)
                };

        };

        var measurements = await BodyMeasurementContext.GetByUserIdAsync(currentUser.UserId);

        measurementWeightLogs = measurements
            .Where(m => m.Weight.HasValue)
            .GroupBy(m => m.MeasurementDate.Date)
            .Select(g =>
            {
                var latest = g.OrderByDescending(m => m.MeasurementDate).First();
                return new WeightLog
                    {
                        Weight = latest.Weight!.Value,
                        Date = DateOnly.FromDateTime(latest.MeasurementDate)
                    };
            })
            .ToList();

        if (startingWeightLog != null && !measurementWeightLogs.Any(w => w.Date == startingWeightLog.Date))
        {
            allWeightLogs.Add(startingWeightLog);
        }


        allWeightLogs.AddRange(measurementWeightLogs);

        allWeightLogs = allWeightLogs
        .OrderBy(w => w.Date)
        .ToList();

    }

    private async Task LoadTodaysFoodLogs()
    {
        if (currentUser != null)
        {
            todaysFoodLogs = await FoodLogService.GetLogsForDate(currentUser.UserId, DateTime.Today);
            caloriesConsumed = todaysFoodLogs.Sum(f => (int)f.NutritionalInfo.Calories);
        }
    }

    private void UpdateWorkoutSchedule()
    {
        if (currentUser?.WorkoutPreferences == null)
        {
            _workoutSchedule = new List<WorkoutPreference>();
            _nextWorkoutPreference = null;
            return;
        }

        _workoutSchedule = currentUser.WorkoutPreferences
            .Where(p => p.IsEnabled)
            .OrderBy(GetNextWorkoutOccurrence)
            .ThenBy(p => p.Type.ToString())
            .ToList();

        _nextWorkoutPreference = _workoutSchedule.FirstOrDefault();
    }

    private DateTime GetNextWorkoutOccurrence(WorkoutPreference preference)
    {
        var today = DateTime.Today;
        var daysAhead = ((int)preference.PreferredDay - (int)today.DayOfWeek + 7) % 7;
        var targetDate = today.AddDays(daysAhead);
        return targetDate.Add(preference.PreferredTime);
    }

    protected string FormatWorkoutDetails(WorkoutPreference preference)
        => $"{preference.PreferredTime:hh\\:mm} · {preference.DurationMinutes} min · Intensity {preference.IntensityLevel}";

    private async Task ShowPredictionDialog()
    {
        var userData = await WeightPredictionService.GetUserDailyData(currentUser.UserId);
        if (userData.Length < 7)
        {
           NotificationService.Notify(NotificationSeverity.Warning, "Insufficient Data", "You need at least 7 days of tracked data to generate a weight prediction.");
           // await DialogService.Alert("Not enough data", "You need at least 7 days of tracked data to generate a weight prediction.");
            return;
        }

        await DialogService.OpenAsync<WeightPrediction>(
            "28-Day Prediction Weight Change",
            new Dictionary<string, object>
                    {
                { "UserId", currentUser?.UserId }
                    },
            new DialogOptions { Width = "700px", CloseDialogOnOverlayClick = true, ShowClose = true }
        );
    }

    private async Task ShowWeightCheckDialog()
    {
        var result = await DialogService.Confirm(
            "Has your weight changed since yesterday?",
            "Weight Check",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" }
        );
        if (result == true)
        {
            await ShowWeightInputDialog();
        }
        else
        {
            var lastWeightLog = allWeightLogs.OrderByDescending(w => w.Date).FirstOrDefault();
            if (lastWeightLog != null)
            {
                var measurement = BodyMeasurement.Create(
                    userId: currentUser.UserId,
                    weight: lastWeightLog.Weight
                );
                await BodyMeasurementContext.AddMeasurement(measurement);
                await LoadUserWeightProgress();
                await LoadDashboardData();
            }
        }
    }

    private async Task ShowWeightInputDialog()
    {
        var result = await DialogService.OpenAsync<WeightInputDialog>(
            "New Weight",
            new Dictionary<string, object>(),
            new DialogOptions { Width = "400px", CloseDialogOnOverlayClick = true }
        );

        if (result is decimal inputWeight && inputWeight > 0)
        {
            var measurement = BodyMeasurement.Create(
                userId: currentUser.UserId,
                weight: inputWeight
            );
            await BodyMeasurementContext.AddMeasurement(measurement);
            await LoadUserWeightProgress();
            await LoadDashboardData();
        }
    }

    public class WeightLog
    {
        public decimal Weight { get; set; }
        public DateOnly Date { get; set; }
    }
    }
}
