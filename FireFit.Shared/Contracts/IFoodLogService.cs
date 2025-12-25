using FireFit.Shared.DTOs;

namespace FireFit.Shared.Contracts;

public interface IFoodLogService
{
    Task SaveFoodLogAsync(FoodLogDto log, CancellationToken ct = default);
    Task<IReadOnlyList<FoodLogDto>> GetUserFoodLogsAsync(string userId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default);
    Task DeleteFoodLogAsync(string id, CancellationToken ct = default);
}

