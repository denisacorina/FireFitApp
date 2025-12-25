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
    public partial class EditFoodLogDialog : ComponentBase
    {
[Parameter] public FoodLog EditableLog { get; set; } = default!;
    private string foodName;
    private float calories;
    private float proteins;
    private float carbs;
    private float fats;
    private MealType? mealType;
    private DateTime timestamp;
    private FoodLog editableLog = default!;
    private MealType[] mealTypes = Enum.GetValues<MealType>();

    protected override void OnInitialized()
    {
        if (EditableLog != null)
        {
            foodName = EditableLog.FoodName;
            calories = EditableLog.NutritionalInfo.Calories;
            proteins = EditableLog.NutritionalInfo.Proteins;
            carbs = EditableLog.NutritionalInfo.Carbs;
            fats = EditableLog.NutritionalInfo.Fats;
            mealType = EditableLog.MealType ?? default;
            timestamp = EditableLog.Timestamp;
        }
    }

    private async Task UpdateLog()
    {
        EditableLog.FoodName = foodName;
        EditableLog.MealType = mealType;
        EditableLog.Timestamp = timestamp;
        EditableLog.NutritionalInfo = NutritionalInfo.Create(calories, proteins, carbs, fats);

        await FoodLogService.UpdateFoodLogAsync(EditableLog);
        DialogService.Close(true);
    }
    }
}
