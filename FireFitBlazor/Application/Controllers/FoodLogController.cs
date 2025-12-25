using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FireFitBlazor.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodLogController(IHttpContextAccessor accessor, IFoodLogService service) : ControllerBase
{
    private readonly IHttpContextAccessor _accessor = accessor;
    private readonly IFoodLogService _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FoodLogDto log)
    {
        await _service.SaveFoodLogAsync(log);
        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IReadOnlyList<FoodLogDto>>> GetList(string userId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var items = await _service.GetUserFoodLogsAsync(userId, from, to);
        return Ok(items);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteFoodLogAsync(id);
        return NoContent();
    }
}

