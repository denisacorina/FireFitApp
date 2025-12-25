using FireFit.Shared.Contracts;
using IGoalService = FireFit.Shared.Contracts.IGoalService;
using FireFit.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FireFitBlazor.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoalsController(IGoalService goals) : ControllerBase
{
    private readonly IGoalService _goals = goals;

    [HttpGet("active/{userId}")]
    public async Task<ActionResult<GoalDto?>> GetActive(string userId)
        => Ok(await _goals.GetActiveGoalAsync(userId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GoalDto goal)
    { await _goals.CreateGoalAsync(goal); return Ok(); }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] GoalDto goal)
    { await _goals.UpdateGoalAsync(goal); return Ok(); }

    [HttpPost("{goalId}/complete")]
    public async Task<IActionResult> Complete(string goalId)
    { await _goals.MarkGoalCompletedAsync(goalId); return Ok(); }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<GoalDto>>> GetUserGoals(string userId)
        => Ok(await _goals.GetUserGoalsAsync(userId));

    [HttpDelete("{goalId}")]
    public async Task<IActionResult> Delete(string goalId)
    { await _goals.DeleteGoalAsync(goalId); return Ok(); }

    [HttpPost("{goalId}/reactivate")]
    public async Task<IActionResult> Reactivate(string goalId)
    { await _goals.ReactivateGoalAsync(goalId); return Ok(); }
}
