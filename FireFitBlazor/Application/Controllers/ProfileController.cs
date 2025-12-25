using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace FireFitBlazor.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController(IProfileService profileService, IAuthService authService) : ControllerBase
{
    private readonly IProfileService _profileService = profileService;
    private readonly IAuthService _authService = authService;

    [HttpGet("me")]
    public async Task<ActionResult<UserDto?>> Get()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user is null) return Unauthorized();
        var profile = await _profileService.GetAsync();
        return Ok(profile ?? user);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProfileUpdateDto dto)
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user is null) return Unauthorized();
        dto.UserId = user.UserId;
        dto.StartingWeight = dto.StartingWeight == 0 ? user.StartingWeight : dto.StartingWeight;
        dto.ProfilePicturePath ??= user.ProfilePicturePath;
        await _profileService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpPost("photo")]
    public async Task<ActionResult<string?>> UploadPhoto([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file provided.");
        var user = await _authService.GetCurrentUserAsync();
        if (user is null) return Unauthorized();
        if (file.Length > 10 * 1024 * 1024) return BadRequest("File too large.");
        await using var stream = file.OpenReadStream();
        var path = await _profileService.UploadPhotoAsync(stream, file.FileName);
        if (string.IsNullOrEmpty(path)) return StatusCode(500, "Failed to upload photo.");
        return Ok(path);
    }
}
