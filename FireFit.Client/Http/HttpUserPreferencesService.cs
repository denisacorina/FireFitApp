using System.Net.Http.Json;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFit.Client.Http;

public class HttpUserPreferencesService(HttpClient http) : IUserPreferencesService
{
    private readonly HttpClient _http = http;

    public Task<UserPreferencesDto?> GetAsync(string userId, CancellationToken ct = default)
        => _http.GetFromJsonAsync<UserPreferencesDto>($"api/preferences/{userId}", ct);

    public Task UpdateAsync(UserPreferencesDto dto, CancellationToken ct = default)
        => _http.PutAsJsonAsync("api/preferences", dto, ct);
}

