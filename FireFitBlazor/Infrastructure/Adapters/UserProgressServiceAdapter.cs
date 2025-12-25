using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;

namespace FireFitBlazor.Infrastructure.Adapters;

public class UserProgressServiceAdapter : IUserProgressService
{
    private readonly IGetUserProgressContext _get;
    private readonly IUpdateUserProgressContext _update;
    public UserProgressServiceAdapter(IGetUserProgressContext get, IUpdateUserProgressContext update)
    {
        _get = get; _update = update;
    }

    public async Task<UserProgressDto?> GetAsync(string userId, CancellationToken ct = default)
    {
        var p = await _get.GetUserProgressAsync(userId);
        if (p == null) return null;
        return new UserProgressDto
        {
            UserId = p.UserId,
            CurrentWeight = p.CurrentWeight,
            StartingWeight = p.StartingWeight,
            UpdatedUtc = p.UpdatedAt
        };
    }

    public async Task UpdateAsync(UserProgressDto progress, CancellationToken ct = default)
    {
        var existing = await _update.GetUserProgressAsync(progress.UserId);
        if (existing == null) return;
        var updated = existing.Update(currentWeight: progress.CurrentWeight);
        await _update.UpdateUserProgressAsync(updated);
    }
}

