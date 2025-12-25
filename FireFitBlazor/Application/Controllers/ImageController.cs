using Microsoft.AspNetCore.Http;
using FireFit.Shared;
using Microsoft.AspNetCore.Mvc;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFitBlazor.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController(IImageRecognitionService svc) : ControllerBase
{
    private readonly IImageRecognitionService _svc = svc;

    [HttpPost("predict")]
    public async Task<ActionResult<IReadOnlyList<IngredientDto>>> Predict([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file");
        using var stream = file.OpenReadStream();
        var result = await _svc.DetectIngredientsAsync(stream);
        return Ok(result);
    }
}
