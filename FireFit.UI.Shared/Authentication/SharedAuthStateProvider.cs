using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace FireFit.UI.Shared.Authentication;

public class SharedAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private readonly bool _fetchOnInit;
    private UserDto? _cachedUser;

    public SharedAuthStateProvider(IAuthService authService, bool fetchOnInit = true)
    {
        _authService = authService;
        _fetchOnInit = fetchOnInit;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            UserDto? user;
            if (!_fetchOnInit && _cachedUser is null)
            {
                user = null;
            }
            else
            {
                user = await EnsureUserAsync();
            }

            if (user is null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            }, "FireFit");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SharedAuthStateProvider] Failed to build auth state: {ex}");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task RefreshAsync()
    {
        _cachedUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        await Task.CompletedTask;
    }

    public void NotifyUserChanged(UserDto? user)
    {
        _cachedUser = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<UserDto?> EnsureUserAsync()
    {
        if (!_fetchOnInit)
        {
            return _cachedUser;
        }

        if (_cachedUser is not null) return _cachedUser;
        await _sync.WaitAsync();
        try
        {
            _cachedUser ??= await _authService.GetCurrentUserAsync();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            Debug.WriteLine("[SharedAuthStateProvider] Current user call returned 401 (unauthorized).");
            _cachedUser = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SharedAuthStateProvider] Failed to fetch current user: {ex}");
            _cachedUser = null;
        }
        finally
        {
            _sync.Release();
        }
        return _cachedUser;
    }
}
