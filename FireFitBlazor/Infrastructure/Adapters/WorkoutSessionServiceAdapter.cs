using System.Linq;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using FireFitBlazor.Application.Services;
using FireFitBlazor.Infrastructure.Adapters;

namespace FireFitBlazor.Infrastructure.Adapters;

public sealed class WorkoutSessionServiceAdapter(IWorkoutService workoutService) : IWorkoutSessionService
{
    private readonly IWorkoutService _workouts = workoutService;

    public async Task<IReadOnlyList<WorkoutSessionDto>> GetUserSessionsAsync(string userId, CancellationToken ct = default)
    {
        var sessions = await _workouts.GetWorkoutsByUserId(userId);
        return sessions.Select(s => s.ToShared()).ToList();
    }

    public async Task<WorkoutSessionDto?> GetByIdAsync(Guid sessionId, CancellationToken ct = default)
    {
        var session = await _workouts.GetWorkoutById(sessionId);
        return session?.ToShared();
    }

    public async Task<WorkoutSessionDto> AddAsync(WorkoutSessionDto session, CancellationToken ct = default)
    {
        var created = await _workouts.AddWorkout(session);
        return created.ToShared();
    }

    public async Task<WorkoutSessionDto> UpdateAsync(WorkoutSessionDto session, CancellationToken ct = default)
    {
        var updated = await _workouts.UpdateWorkout(session);
        return updated.ToShared();
    }

    public Task DeleteAsync(Guid sessionId, CancellationToken ct = default)
        => _workouts.DeleteWorkout(sessionId);
}
