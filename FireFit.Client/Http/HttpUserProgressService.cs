using System.Net.Http.Json;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFit.Client.Http;

public class HttpUserProgressService(HttpClient http) : IUserProgressService
{
    private readonly HttpClient _http = http;

    public Task<UserProgressDto?> GetAsync(string userId, CancellationToken ct = default)
        => _http.GetFromJsonAsync<UserProgressDto>($"api/progress/{userId}", ct);

    public Task UpdateAsync(UserProgressDto progress, CancellationToken ct = default)
        => _http.PutAsJsonAsync("api/progress", progress, ct);
}

