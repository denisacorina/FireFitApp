using System.Diagnostics;
using System.Net.Http.Json;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFit.Client.Http;

public class HttpAuthService(HttpClient http) : IAuthService
{
    private readonly HttpClient _http = http;

    public async Task<UserDto?> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("api/customauth/register", dto, ct);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"[HttpAuthService] Register failed: {ex.Message}");
            throw;
        }
    }

    public async Task<UserDto?> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("api/customauth/login", dto, ct);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"[HttpAuthService] Login failed: {ex.Message}");
            throw;
        }
    }

    public Task LogoutAsync(CancellationToken ct = default)
        => _http.PostAsync("api/customauth/logout", null, ct);

    public async Task<UserDto?> GetCurrentUserAsync(CancellationToken ct = default)
    {
        try
        {
            var res = await _http.GetAsync("api/customauth/me", ct);
            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return null;
            }

            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"[HttpAuthService] Fetch current user failed: {ex.Message}");
            throw;
        }
    }
}
