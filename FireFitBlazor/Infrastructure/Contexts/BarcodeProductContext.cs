using FireFitBlazor.Domain.Interfaces;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.ValueObjects;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FireFitBlazor.Infrastructure.Contexts
{
    public class BarcodeProductContext : IBarcodeProductContext
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _openFoodFactsApiUrl = "https://world.openfoodfacts.org/api/v0/product/";

        public BarcodeProductContext(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        



        //public async Task<BarcodeProduct> GetBarcodeProductAsync(string barcode)
        //{
        //    // First, try to find the product in our database
        //    var product = await _context.BarcodeProducts
        //        .FirstOrDefaultAsync(p => p.BarcodeNumber == barcode);

        //    if (product != null)
        //        return product;

        //    // If not found, fetch from Open Food Facts API
        //    try
        //    {
        //        var response = await _httpClient.GetAsync($"{_openFoodFactsApiUrl}{barcode}.json");
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            var result = JsonSerializer.Deserialize<OpenFoodFactsResponse>(content);

        //            if (result?.Status == 1 && result.Product != null)
        //            {
        //                var newProduct = new BarcodeProduct
        //                {
        //                    BarcodeNumber = barcode,
        //                    ProductName = result.Product.ProductName,
        //                    Nutrition = new NutritionalInfo
        //                    {
        //                        Calories = (int)(result.Product.Nutriments?.EnergyKcal100g ?? 0),
        //                        Proteins = (decimal)(result.Product.Nutriments?.Proteins100g ?? 0),
        //                        Carbs = (decimal)(result.Product.Nutriments?.Carbohydrates100g ?? 0),
        //                        Fats = (decimal)(result.Product.Nutriments?.Fat100g ?? 0)
        //                    }
        //                };

        //                // Save to our database for future use
        //                _context.BarcodeProducts.Add(newProduct);
        //                await _context.SaveChangesAsync();

        //                return newProduct;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error
        //        // TODO: Add proper logging
        //    }

        //    return null;
        //}

        private class OpenFoodFactsResponse
        {
            public int Status { get; set; }
            public OpenFoodFactsProduct Product { get; set; }
        }

        private class OpenFoodFactsProduct
        {
            public string ProductName { get; set; }
            public OpenFoodFactsNutriments Nutriments { get; set; }
        }

        private class OpenFoodFactsNutriments
        {
            public double? EnergyKcal100g { get; set; }
            public double? Proteins100g { get; set; }
            public double? Carbohydrates100g { get; set; }
            public double? Fat100g { get; set; }
        }
    }
} 