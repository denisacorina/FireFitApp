using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FireFitBlazor.Application.Services;

namespace FireFitBlazor.Application
{
    public partial class FoodLogEntryChoice : ComponentBase
    {
[Parameter]
    [SupplyParameterFromQuery]
    public string? MealType { get; set; }

    private string mealType = "breakfast";
    private bool isLoading = false;
    private async Task OnImageSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        try
        {
            mealType = MealType ?? mealType;
            isLoading = true;
            StateHasChanged();
            using var stream = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024).CopyToAsync(stream);
            var base64 = Convert.ToBase64String(stream.ToArray());
            // Store the image in a shared service or state
            ImageTransferService.CapturedImage = $"data:{file.ContentType};base64,{base64}";
            await Task.Delay(50);
            Navigation.NavigateTo($"/meal-detect-demo?mealType={mealType}");
        }
        catch(Exception ex)
        {
            isLoading = false;
            StateHasChanged();
        }
    }
    }
}

