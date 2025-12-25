using System.Net.Http.Headers;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFitBlazor.Infrastructure.Adapters;

public class ImageRecognitionServiceAdapter : IImageRecognitionService
{
    private readonly IHttpClientFactory _httpFactory;
    public ImageRecognitionServiceAdapter(IHttpClientFactory httpFactory) => _httpFactory = httpFactory;

    public async Task<IReadOnlyList<IngredientDto>> DetectIngredientsAsync(Stream imageStream, CancellationToken ct = default)
    {
        var client = _httpFactory.CreateClient("MLAPI");
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(imageStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(streamContent, "file", "image.png");
        var response = await client.PostAsync("predict", content, ct);
        if (!response.IsSuccessStatusCode) return Array.Empty<IngredientDto>();
        var json = await response.Content.ReadAsStringAsync(ct);
        // Expecting { predictedLabel: string, score: number }
        try
        {
            var doc = System.Text.Json.JsonDocument.Parse(json);
            var label = doc.RootElement.GetProperty("predictedLabel").GetString() ?? string.Empty;
            var score = doc.RootElement.TryGetProperty("score", out var s) ? s.GetSingle() : 0f;
            if (string.IsNullOrWhiteSpace(label) || score < 0.25f) return Array.Empty<IngredientDto>();
            return new [] { new IngredientDto { Name = label, QuantityGrams = 100 } };
        }
        catch { return Array.Empty<IngredientDto>(); }
    }
}

