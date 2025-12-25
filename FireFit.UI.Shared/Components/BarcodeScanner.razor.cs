using Radzen;
using Radzen.Blazor;
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
using FireFitBlazor.Application.Services;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Enums;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFit.UI.Shared.Components
{
    public partial class BarcodeScanner : ComponentBase
    {
[Parameter]
    public string? UserId { get; set; }

    [Parameter]
    public MealType? MealType { get; set; }

    private FoodProduct? ScannedProduct { get; set; }
    private float ConsumedQuantityInGrams { get; set; } = 100;
    private string? errorMessage;

    private float AdjustedKcal => (ScannedProduct?.Nutriments?.Energy_kcal_100g ?? 0) * ConsumedQuantityInGrams / 100;
    private float AdjustedProteins => (ScannedProduct?.Nutriments?.Proteins_100g ?? 0) * ConsumedQuantityInGrams / 100;
    private float AdjustedCarbs => (ScannedProduct?.Nutriments?.Carbohydrates_100g ?? 0) * ConsumedQuantityInGrams / 100;
    private float AdjustedFats => (ScannedProduct?.Nutriments?.Fat_100g ?? 0) * ConsumedQuantityInGrams / 100;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //await JS.InvokeVoidAsync("barcodeScanner.init", DotNetObjectReference.Create(this));
            //await JS.InvokeVoidAsync("barcodeScanner.captureOnce", DotNetObjectReference.Create(this));
            await JS.InvokeVoidAsync("barcodeScannerManual.startCamera", DotNetObjectReference.Create(this));
        }
    }

    [JSInvokable]
    private async Task CaptureBarcode()
    {
        await JS.InvokeVoidAsync("barcodeScannerManual.captureBarcode");
    }

    [JSInvokable]
    public async Task OnBarcodeDetected(string barcode)
    {
        try
        {
            ScannedProduct = await OpenFoodFactsService.GetProductByBarcodeAsync(barcode);

            if (ScannedProduct == null)
            {
                errorMessage = $"No product found for barcode: {barcode}";
            }

                ConsumedQuantityInGrams = 100;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            errorMessage = "Error processing barcode: " + ex.Message;
            StateHasChanged();
        }
    }

    [JSInvokable]
    private async Task RestartScanner()
    {
        ScannedProduct = null;
        errorMessage = null;

        await JS.InvokeVoidAsync("barcodeScannerManual.stop");
        await JS.InvokeVoidAsync("barcodeScannerManual.startCamera", DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public Task OnScannerError(string error)
    {
        errorMessage = "Camera error: " + error;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task AddToFoodLog()
    {
        if (ScannedProduct == null || string.IsNullOrEmpty(UserId) || MealType == null)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Error", "Missing required information");
            return;
        }

        try
        {
            var kcal100 = ScannedProduct.Nutriments?.Energy_kcal_100g ?? 0;
            var protein100 = ScannedProduct.Nutriments?.Proteins_100g ?? 0;
            var carb100 = ScannedProduct.Nutriments?.Carbohydrates_100g ?? 0;
            var fat100 = ScannedProduct.Nutriments?.Fat_100g ?? 0;

            var log = FoodLog.Create(
                 userId: UserId,
                 foodName: ScannedProduct.Product_name,
                 calories: (int)AdjustedKcal,
                 proteins: AdjustedProteins,
                 carbs: AdjustedCarbs,
                 fats: AdjustedFats,
                 mealType: MealType.Value
                 );

            await FoodLogService.SaveFoodLogAsync(log);
            NotificationService.Notify(NotificationSeverity.Success, "Success", "Food added to log");
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Error", "Failed to add food to log: " + ex.Message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (JS is not null)
        {
            try
            {
                await JS.InvokeVoidAsync("barcodeScannerManual.stop");
            }
            catch (JSDisconnectedException)
            {
                // Circuit already disposed, ignore
            }
        }
    }
    }
}

