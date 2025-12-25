using FireFit.Shared.DTOs;

namespace FireFit.Shared.Contracts;

public interface IAuthService
{
    Task<UserDto?> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<UserDto?> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task LogoutAsync(CancellationToken ct = default);
    Task<UserDto?> GetCurrentUserAsync(CancellationToken ct = default);
}
