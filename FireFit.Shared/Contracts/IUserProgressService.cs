using FireFit.Shared.DTOs;

namespace FireFit.Shared.Contracts;

public interface IUserProgressService
{
    Task<UserProgressDto?> GetAsync(string userId, CancellationToken ct = default);
    Task UpdateAsync(UserProgressDto progress, CancellationToken ct = default);
}

