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
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Enums;
using FireFitBlazor.Application.Services;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace FireFitBlazor.Components
{
    public partial class QuickAddDialog : ComponentBase
    {
[Parameter] public string UserId { get; set; }
    [Parameter] public string MealType { get; set; }

    private QuickAddModel model = new();

    private class QuickAddModel
    {
        public string FoodName { get; set; }
        public float Calories { get; set; }
        public float? Proteins { get; set; }
        public float? Carbs { get; set; }
        public float? Fats { get; set; }
        public float Quantity { get; set; } = 100;
    }

    private async Task OnSubmit()
    {
        try
        {
            var mealTypeEnum = MealType.ToLower() switch
            {
                "breakfast" => FoodTrackingEnums.MealType.Breakfast,
                "lunch" => FoodTrackingEnums.MealType.Lunch,
                "dinner" => FoodTrackingEnums.MealType.Dinner,
                _ => FoodTrackingEnums.MealType.Breakfast
            };

            var log = FoodLog.Create(
                userId: UserId,
                foodName: model.FoodName,
                calories: (int)(model.Calories * model.Quantity / 100),
                proteins: model.Proteins * model.Quantity / 100 ?? 0,
                carbs: model.Carbs * model.Quantity / 100 ?? 0,
                fats: model.Fats * model.Quantity / 100 ?? 0,
                mealType: mealTypeEnum
            );

            await FoodLogService.SaveFoodLogAsync(log);
            DialogService.Close(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving food log: {ex.Message}");
            DialogService.Close(false);
        }
    }
    }
}
