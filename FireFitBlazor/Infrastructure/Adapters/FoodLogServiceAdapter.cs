using FireFit.Shared.DTOs;

namespace FireFitBlazor.Infrastructure.Adapters;

public class FoodLogServiceAdapter : FireFit.Shared.Contracts.IFoodLogService
{
    private readonly FireFitBlazor.Application.Services.IFoodLogService _inner;
    public FoodLogServiceAdapter(FireFitBlazor.Application.Services.IFoodLogService inner)
    {
        _inner = inner;
    }

    public async Task SaveFoodLogAsync(FoodLogDto log, CancellationToken ct = default)
    {
        var domain = log.ToDomain();
        await _inner.SaveFoodLogAsync(domain);
    }

    public async Task<IReadOnlyList<FoodLogDto>> GetUserFoodLogsAsync(string userId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default)
    {
        var from = fromUtc?.Date ?? DateTime.UtcNow.Date;
        var logs = await _inner.GetLogsForDate(userId, from);
        return logs.Select(x => x.ToShared()).ToList();
    }

    public async Task DeleteFoodLogAsync(string id, CancellationToken ct = default)
    {
        if (Guid.TryParse(id, out var gid))
        {
            await _inner.DeleteLog(gid);
        }
    }
}
