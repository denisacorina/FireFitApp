using FireFitBlazor.Application.Services;
using FireFitBlazor.Domain.Enums;
using FireFitBlazor.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Components;

public partial class BarcodeScanner : IAsyncDisposable
{
    [Parameter] public string? UserId { get; set; }
    [Parameter] public MealType? MealType { get; set; }

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
            await JS.InvokeVoidAsync("barcodeScannerManual.startCamera", DotNetObjectReference.Create(this));
        }
    }

    [JSInvokable]
    private Task CaptureBarcode()
        => JS.InvokeVoidAsync("barcodeScannerManual.captureBarcode").AsTask();

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
            var log = FoodLog.Create(
                 userId: UserId,
                 foodName: ScannedProduct.Product_name,
                 calories: (int)AdjustedKcal,
                 proteins: AdjustedProteins,
                 carbs: AdjustedCarbs,
                 fats: AdjustedFats,
                 mealType: MealType.Value);

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
            }
        }
    }

    [Inject] private OpenFoodFactsService OpenFoodFactsService { get; set; } = default!;
    [Inject] private IFoodLogService FoodLogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
}
