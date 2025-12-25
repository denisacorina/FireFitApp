using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FireFitBlazor.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController(IUserProgressService svc) : ControllerBase
{
    private readonly IUserProgressService _svc = svc;

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserProgressDto?>> Get(string userId)
        => Ok(await _svc.GetAsync(userId));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserProgressDto dto)
    { await _svc.UpdateAsync(dto); return Ok(); }
}

