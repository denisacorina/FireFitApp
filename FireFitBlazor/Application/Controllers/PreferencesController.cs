using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FireFitBlazor.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreferencesController(IUserPreferencesService svc) : ControllerBase
{
    private readonly IUserPreferencesService _svc = svc;

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserPreferencesDto?>> Get(string userId)
        => Ok(await _svc.GetAsync(userId));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserPreferencesDto dto)
    { await _svc.UpdateAsync(dto); return Ok(); }
}

