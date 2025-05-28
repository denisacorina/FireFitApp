using FireFitBlazor.Domain.Interfaces;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Services;
using FireFitBlazor.Domain.ValueObjects;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FireFitBlazor.Infrastructure.Contexts
{
    public class IngredientRecognitionContext : IIngredientRecognitionContext
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiEndpoint;
        private readonly ILoggingService _loggingService;

        public IngredientRecognitionContext(HttpClient httpClient, string apiEndpoint, ILoggingService loggingService)
        {
            _httpClient = httpClient;
            _apiEndpoint = apiEndpoint;
            _loggingService = loggingService;
        }

        //public async Task<FoodLog> AnalyzeImageAsync(string imageBase64)
        //{
        //    try
        //    {
        //        _loggingService.LogInformation("Starting image analysis");

        //        // Validate input
        //        if (string.IsNullOrWhiteSpace(imageBase64))
        //        {
        //            _loggingService.LogWarning("Empty image data received");
        //            throw new ArgumentException("Image data cannot be empty", nameof(imageBase64));
        //        }

        //        // Prepare the request payload
        //        var requestData = new
        //        {
        //            image = imageBase64
        //        };

        //        var content = new StringContent(
        //            JsonSerializer.Serialize(requestData),
        //            Encoding.UTF8,
        //            "application/json"
        //        );

        //        // Send the request to your external image recognition service
        //        var response = await _httpClient.PostAsync(_apiEndpoint, content);
                
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var errorContent = await response.Content.ReadAsStringAsync();
        //            _loggingService.LogError(new Exception($"API returned status code: {response.StatusCode}"), 
        //                $"Failed to analyze image. Response: {errorContent}");
        //            throw new Exception($"Image recognition service returned status code: {response.StatusCode}");
        //        }

        //        // Parse the response
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var result = JsonSerializer.Deserialize<ImageRecognitionResult>(responseContent);

        //        if (result == null)
        //        {
        //            _loggingService.LogError(new Exception("Null response from API"), 
        //                "Failed to deserialize image recognition response");
        //            throw new Exception("Invalid response from image recognition service");
        //        }

        //        // Convert the recognition result to a FoodLog
        //        var foodLog = new FoodLog
        //        {
        //            FoodName = result.FoodName,
        //            NutritionalInfo = new NutritionalInfo
        //            {
        //                Calories = result.Calories,
        //                Proteins = result.Proteins,
        //                Carbs = result.Carbs,
        //                Fats = result.Fats
        //            }
        //        };

        //        _loggingService.LogInformation($"Successfully analyzed image. Recognized food: {result.FoodName}");
        //        return foodLog;
        //    }
        //    catch (Exception ex)
        //    {
        //        _loggingService.LogError(ex, "Failed to analyze image");
        //        throw;
        //    }
        //}

        private class ImageRecognitionResult
        {
            public string FoodName { get; set; }
            public int Calories { get; set; }
            public decimal Proteins { get; set; }
            public decimal Carbs { get; set; }
            public decimal Fats { get; set; }
        }
    }
} 