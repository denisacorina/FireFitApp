using FireFit.Shared.DTOs;

namespace FireFit.Shared.Contracts;

public interface IBodyMeasurementService
{
    Task AddAsync(BodyMeasurementDto m, CancellationToken ct = default);
    Task<IReadOnlyList<BodyMeasurementDto>> GetListAsync(string userId, CancellationToken ct = default);
    Task<BodyMeasurementDto?> GetLatestAsync(string userId, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}

