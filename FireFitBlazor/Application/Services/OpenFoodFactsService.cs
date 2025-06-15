using System.Net.Http.Json;
using System.Text.Json;

namespace FireFitBlazor.Application.Services;

public class OpenFoodFactsService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://world.openfoodfacts.org/api/v0";

    public OpenFoodFactsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<FoodProduct?> GetProductByBarcodeAsync(string barcode)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<OpenFoodFactsResponse>($"{BaseUrl}/product/{barcode}.json");
            return response?.Product;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

public class OpenFoodFactsResponse
{
    public int Status { get; set; }
    public string Status_verbose { get; set; } = string.Empty;
    public FoodProduct? Product { get; set; }
}

public class FoodProduct
{
    public string Code { get; set; } = string.Empty;
    public string Product_name { get; set; } = string.Empty;
    public string Brands { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public Nutriments? Nutriments { get; set; }
    public string Image_url { get; set; } = string.Empty;
}

public class Nutriments
{
    public float? Energy_kcal_100g { get; set; }
    public float? Proteins_100g { get; set; }
    public float? Carbohydrates_100g { get; set; }
    public float? Fat_100g { get; set; }
} 