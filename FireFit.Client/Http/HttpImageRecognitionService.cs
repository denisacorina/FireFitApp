using System.Net.Http.Headers;
using System.Net.Http.Json;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFit.Client.Http;

public class HttpImageRecognitionService(HttpClient http) : IImageRecognitionService
{
    private readonly HttpClient _http = http;

    public async Task<IReadOnlyList<IngredientDto>> DetectIngredientsAsync(Stream imageStream, CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(imageStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(streamContent, "file", "image.png");
        var res = await _http.PostAsync("api/image/predict", content, ct);
        res.EnsureSuccessStatusCode();
        var result = await res.Content.ReadFromJsonAsync<IReadOnlyList<IngredientDto>>(cancellationToken: ct);
        return result ?? Array.Empty<IngredientDto>();
    }
}
