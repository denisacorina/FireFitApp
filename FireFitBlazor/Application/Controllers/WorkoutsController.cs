using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FireFitBlazor.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutsController(IWorkoutSessionService workoutService) : ControllerBase
{
    private readonly IWorkoutSessionService _workouts = workoutService;

    private string? CurrentUserId =>
        User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? Request.Cookies["userId"];

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WorkoutSessionDto>>> Get([FromQuery] string? userId = null)
    {
        var current = CurrentUserId;
        if (current is null)
        {
            return Unauthorized();
        }

        var effective = string.IsNullOrWhiteSpace(userId) ? current : userId;
        if (!string.Equals(effective, current, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        var sessions = await _workouts.GetUserSessionsAsync(effective);
        return Ok(sessions);
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<WorkoutSessionDto>> GetById(Guid sessionId)
    {
        var current = CurrentUserId;
        if (current is null)
        {
            return Unauthorized();
        }

        var session = await _workouts.GetByIdAsync(sessionId);
        if (session is null)
        {
            return NotFound();
        }

        if (!string.Equals(session.UserId, current, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        return Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutSessionDto>> Post([FromBody] WorkoutSessionDto session)
    {
        var current = CurrentUserId;
        if (current is null)
        {
            return Unauthorized();
        }

        session.UserId = current;
        var created = await _workouts.AddAsync(session);
        return Ok(created);
    }

    [HttpPut("{sessionId:guid}")]
    public async Task<ActionResult<WorkoutSessionDto>> Put(Guid sessionId, [FromBody] WorkoutSessionDto session)
    {
        var current = CurrentUserId;
        if (current is null)
        {
            return Unauthorized();
        }

        var existing = await _workouts.GetByIdAsync(sessionId);
        if (existing is null)
        {
            return NotFound();
        }

        if (!string.Equals(existing.UserId, current, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        session.SessionId = sessionId;
        session.UserId = current;
        var updated = await _workouts.UpdateAsync(session);
        return Ok(updated);
    }

    [HttpDelete("{sessionId:guid}")]
    public async Task<IActionResult> Delete(Guid sessionId)
    {
        var current = CurrentUserId;
        if (current is null)
        {
            return Unauthorized();
        }

        var existing = await _workouts.GetByIdAsync(sessionId);
        if (existing is null)
        {
            return NotFound();
        }

        if (!string.Equals(existing.UserId, current, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        await _workouts.DeleteAsync(sessionId);
        return NoContent();
    }
}
