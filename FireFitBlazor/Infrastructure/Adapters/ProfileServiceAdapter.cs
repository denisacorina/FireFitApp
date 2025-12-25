using System.Linq;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using FireFitBlazor.Domain.ContextInterfaces.UserContexts.User;
using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Adapters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FireFitBlazor.Infrastructure.Adapters;

public class ProfileServiceAdapter : IProfileService
{
    private readonly IGetUserContext _getUser;
    private readonly IUpdateUserContext _updateUser;
    private readonly IUserPreferencesContext _preferences;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _environment;

    public ProfileServiceAdapter(
        IGetUserContext getUser,
        IUpdateUserContext updateUser,
        IUserPreferencesContext preferences,
        IWebHostEnvironment environment,
        IHttpContextAccessor httpContextAccessor)
    {
        _getUser = getUser;
        _updateUser = updateUser;
        _preferences = preferences;
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserDto?> GetAsync(CancellationToken ct = default)
    {
        var userId = GetUserIdFromContext();
        if (userId is null) return null;
        var user = await GetInternalAsync(userId);
        if (user is null) return null;
        var workoutPreferences = await _preferences.GetWorkoutPreferencesAsync(userId);
        var mapped = workoutPreferences.Select(p => p.ToShared()).ToList();
        return user with { WorkoutPreferences = mapped, WorkoutTypes = mapped.Select(m => m.Type).ToList() };
    }

    public async Task UpdateAsync(ProfileUpdateDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            var current = GetUserIdFromContext();
            if (current is null) return;
            dto.UserId = current;
        }

        var user = await _getUser.Execute(dto.UserId);
        if (user == null) return;

        await _updateUser.Execute(
            dto.UserId,
            user.Email,
            dto.Name,
            dto.Age,
            dto.Height,
            dto.StartingWeight == 0 ? user.StartingWeight : dto.StartingWeight,
            dto.TargetWeight == 0 ? user.TargetWeight : dto.TargetWeight,
            dto.WeightGoal.ToDomain(),
            dto.ActivityLevel.ToDomain(),
            dto.DietaryPreferences.Select(d => d.ToDomain()).ToList(),
            dto.WorkoutTypes.Select(w => w.ToDomain()).ToList(),
            dto.ProfilePicturePath ?? user.ProfilePicturePath ?? string.Empty,
            dto.FitnessExperience.ToDomain());

        await _preferences.UpdateDietaryPreferencesAsync(dto.UserId, dto.DietaryPreferences.Select(d => d.ToDomain()).ToList());

        var preferenceDtos = (dto.WorkoutPreferences?.Any() == true
            ? dto.WorkoutPreferences
            : dto.WorkoutTypes.Select(t => new WorkoutPreferenceDto { Type = t })).ToList();

        var workoutPreferences = preferenceDtos.Select(p =>
            WorkoutPreference.Create(
                dto.UserId,
                p.Type.ToDomain(),
                p.PreferredDay,
                p.PreferredTime,
                p.DurationMinutes,
                p.IntensityLevel)).ToList();

        dto.WorkoutTypes = workoutPreferences.Select(p => p.Type.ToShared()).ToList();
        await _preferences.UpdateWorkoutPreferencesAsync(dto.UserId, workoutPreferences);
    }

    public async Task<string?> UploadPhotoAsync(Stream stream, string fileName, CancellationToken ct = default)
    {
        var userId = GetUserIdFromContext();
        if (userId is null) return null;

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".png";
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? string.Empty, "uploads", "profiles", userId);
        Directory.CreateDirectory(uploadsFolder);
        var file = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}{extension}");
        await using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await stream.CopyToAsync(fs, ct);
        }

        var relative = Path.GetRelativePath(_environment.WebRootPath ?? string.Empty, file)
            .Replace('\\', '/');
        return "/" + relative.TrimStart('/');
    }

    private string? GetUserIdFromContext()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies["userId"]
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }

    private async Task<UserDto?> GetInternalAsync(string userId)
    {
        var user = await _getUser.Execute(userId);
        return user?.ToShared();
    }
}
