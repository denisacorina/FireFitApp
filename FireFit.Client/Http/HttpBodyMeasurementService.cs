using System.Net.Http.Json;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFit.Client.Http;

public class HttpBodyMeasurementService(HttpClient http) : IBodyMeasurementService
{
    private readonly HttpClient _http = http;

    public Task AddAsync(BodyMeasurementDto m, CancellationToken ct = default)
        => _http.PostAsJsonAsync("api/measurements", m, ct);

    public Task<IReadOnlyList<BodyMeasurementDto>> GetListAsync(string userId, CancellationToken ct = default)
        => _http.GetFromJsonAsync<IReadOnlyList<BodyMeasurementDto>>($"api/measurements/{userId}", ct)!;

    public Task<BodyMeasurementDto?> GetLatestAsync(string userId, CancellationToken ct = default)
        => _http.GetFromJsonAsync<BodyMeasurementDto>($"api/measurements/{userId}/latest", ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"api/measurements/{id}", ct);
}

