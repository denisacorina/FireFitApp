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
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Application
{
    public partial class WeightPrediction : ComponentBase
    {
[Parameter] public string UserId { get; set; }
    private WeightPredictionService.WeightPredictionWithAnalysis prediction;
    private WeightPredictionService.WeightChangeLimit maxWeightChange;

    private bool isLoading = true;

    private class WeightSummaryItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    private User? currentUser;


    protected override async Task OnInitializedAsync()
    {
        isLoading = true;
        try
        {
            prediction = await WeightPredictionService.PredictWeight28Days(UserId);
            maxWeightChange = WeightPredictionService.CalculateMaxSafeWeightChange((float)prediction.CurrentWeight, 28);
        }
        catch (Exception ex)
        {
            DialogService.Close();
        }
        finally 
        {
            isLoading = false;
        }
    }

    private List<WeightSummaryItem> GetWeightSummary()
    {
        return new List<WeightSummaryItem>
        {
            new() { Label = "Current Weight", Value = $"{prediction.CurrentWeight:F1} kg" },
            new() { Label = "Predicted Weight (Day 28)", Value = $"{prediction.PredictedWeight:F1} kg" },
            new() { Label = "Expected Change", Value = $"{(prediction.PredictedWeight - prediction.CurrentWeight):F1} kg" },
            new() { Label = "Prediction Date", Value = $"{prediction.PredictionDate:d}" }
        };
    }

    private AlertStyle GetBehaviorAlertStyle(BehaviorAnalysis analysis)
    {
        return analysis.IsAlignedWithGoal ? AlertStyle.Success :
               analysis.IsPlateauing ? AlertStyle.Warning :
               analysis.IsOvereating ? AlertStyle.Danger :
               AlertStyle.Info;
    }

    private string GetActivityLevelDescription(ActivityMetrics metrics)
    {
        return metrics.AverageLevel switch
        {
            var x when x >= 4.0f => "Very High",
            var x when x >= 3.0f => "High",
            var x when x >= 2.0f => "Moderate",
            var x when x >= 1.0f => "Low",
            _ => "Very Low"
        };
    }
    }
}
