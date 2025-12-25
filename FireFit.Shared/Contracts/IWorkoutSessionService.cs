using FireFit.Shared.DTOs;

namespace FireFit.Shared.Contracts;

public interface IWorkoutSessionService
{
    Task<IReadOnlyList<WorkoutSessionDto>> GetUserSessionsAsync(string userId, CancellationToken ct = default);
    Task<WorkoutSessionDto?> GetByIdAsync(Guid sessionId, CancellationToken ct = default);
    Task<WorkoutSessionDto> AddAsync(WorkoutSessionDto session, CancellationToken ct = default);
    Task<WorkoutSessionDto> UpdateAsync(WorkoutSessionDto session, CancellationToken ct = default);
    Task DeleteAsync(Guid sessionId, CancellationToken ct = default);
}
