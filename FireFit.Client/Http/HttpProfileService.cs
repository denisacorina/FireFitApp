using System.Net.Http.Headers;
using System.Net.Http.Json;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFit.Client.Http;

public class HttpProfileService(HttpClient http) : IProfileService
{
    private readonly HttpClient _http = http;

    public Task<UserDto?> GetAsync(CancellationToken ct = default)
        => _http.GetFromJsonAsync<UserDto>("api/profile/me", ct);

    public Task UpdateAsync(ProfileUpdateDto dto, CancellationToken ct = default)
        => _http.PutAsJsonAsync("api/profile", dto, ct);

    public async Task<string?> UploadPhotoAsync(Stream stream, string fileName, CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        var response = await _http.PostAsync("api/profile/photo", content, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<string>(cancellationToken: ct);
    }
}
