using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FireFitBlazor.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsController(IBodyMeasurementService svc) : ControllerBase
{
    private readonly IBodyMeasurementService _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BodyMeasurementDto dto)
    { await _svc.AddAsync(dto); return Ok(); }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IReadOnlyList<BodyMeasurementDto>>> GetList(string userId)
        => Ok(await _svc.GetListAsync(userId));

    [HttpGet("{userId}/latest")]
    public async Task<ActionResult<BodyMeasurementDto?>> GetLatest(string userId)
        => Ok(await _svc.GetLatestAsync(userId));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    { await _svc.DeleteAsync(id); return NoContent(); }
}

