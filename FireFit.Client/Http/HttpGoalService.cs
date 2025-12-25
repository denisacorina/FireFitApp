using System.Net.Http.Json;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFit.Client.Http;

public class HttpGoalService(HttpClient http) : IGoalService
{
    private readonly HttpClient _http = http;

    public Task<GoalDto?> GetActiveGoalAsync(string userId, CancellationToken ct = default)
        => _http.GetFromJsonAsync<GoalDto>($"api/goals/active/{userId}", ct);

    public Task CreateGoalAsync(GoalDto goal, CancellationToken ct = default)
        => _http.PostAsJsonAsync("api/goals", goal, ct);

    public Task UpdateGoalAsync(GoalDto goal, CancellationToken ct = default)
        => _http.PutAsJsonAsync("api/goals", goal, ct);

    public Task MarkGoalCompletedAsync(string goalId, CancellationToken ct = default)
        => _http.PostAsync($"api/goals/{goalId}/complete", null, ct);

    public async Task<IEnumerable<GoalDto>> GetUserGoalsAsync(string userId, CancellationToken ct = default)
    {
        var res = await _http.GetFromJsonAsync<IEnumerable<GoalDto>>($"api/goals/user/{userId}", ct);
        return res ?? Array.Empty<GoalDto>();
    }

    public Task DeleteGoalAsync(string goalId, CancellationToken ct = default)
        => _http.DeleteAsync($"api/goals/{goalId}", ct);

    public Task ReactivateGoalAsync(string goalId, CancellationToken ct = default)
        => _http.PostAsync($"api/goals/{goalId}/reactivate", null, ct);
}

