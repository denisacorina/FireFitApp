using System.Net.Http.Json;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFit.Client.Http;

public class HttpFoodLogService(HttpClient http) : IFoodLogService
{
    private readonly HttpClient _http = http;

    public Task SaveFoodLogAsync(FoodLogDto log, CancellationToken ct = default)
        => _http.PostAsJsonAsync("api/foodlog", log, ct);

    public Task<IReadOnlyList<FoodLogDto>> GetUserFoodLogsAsync(string userId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default)
        => _http.GetFromJsonAsync<IReadOnlyList<FoodLogDto>>($"api/foodlog/{userId}?from={fromUtc:O}&to={toUtc:O}", ct)!
            .ContinueWith(t => (IReadOnlyList<FoodLogDto>) (t.Result ?? new List<FoodLogDto>()), ct);

    public Task DeleteFoodLogAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"api/foodlog/{id}", ct);
}

