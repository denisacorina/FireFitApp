using FireFit.Shared.DTOs;

namespace FireFit.Shared.Contracts;

public interface IUserPreferencesService
{
    Task<UserPreferencesDto?> GetAsync(string userId, CancellationToken ct = default);
    Task UpdateAsync(UserPreferencesDto dto, CancellationToken ct = default);
}

