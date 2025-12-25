using System.Net.Http.Json;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFit.Client.Http;

public sealed class HttpWorkoutSessionService(HttpClient http) : IWorkoutSessionService
{
    private readonly HttpClient _http = http;

    public async Task<IReadOnlyList<WorkoutSessionDto>> GetUserSessionsAsync(string userId, CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<IReadOnlyList<WorkoutSessionDto>>($"api/workouts?userId={userId}", ct);
        return result ?? Array.Empty<WorkoutSessionDto>();
    }

    public Task<WorkoutSessionDto?> GetByIdAsync(Guid sessionId, CancellationToken ct = default)
        => _http.GetFromJsonAsync<WorkoutSessionDto>($"api/workouts/{sessionId}", ct);

    public async Task<WorkoutSessionDto> AddAsync(WorkoutSessionDto session, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/workouts", session, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkoutSessionDto>(cancellationToken: ct)
               ?? throw new InvalidOperationException("Workout response payload was empty.");
    }

    public async Task<WorkoutSessionDto> UpdateAsync(WorkoutSessionDto session, CancellationToken ct = default)
    {
        if (session.SessionId is null)
        {
            throw new ArgumentException("SessionId must be provided for updates.", nameof(session));
        }

        var response = await _http.PutAsJsonAsync($"api/workouts/{session.SessionId}", session, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkoutSessionDto>(cancellationToken: ct)
               ?? throw new InvalidOperationException("Workout response payload was empty.");
    }

    public async Task DeleteAsync(Guid sessionId, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"api/workouts/{sessionId}", ct);
        response.EnsureSuccessStatusCode();
    }
}
