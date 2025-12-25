using BCrypt.Net;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Adapters;
using DomainEnums = FireFitBlazor.Domain.Enums;

namespace FireFitBlazor.Infrastructure.Adapters;

using SharedRegisterDto = FireFit.Shared.DTOs.RegisterDto;
using SharedLoginDto = FireFit.Shared.DTOs.LoginDto;

public class AuthServiceAdapter : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;
    private readonly FireFitBlazor.Domain.ContextInterfaces.IGoalContext _goals;

    public AuthServiceAdapter(ApplicationDbContext db, IHttpContextAccessor http, FireFitBlazor.Domain.ContextInterfaces.IGoalContext goals)
    {
        _db = db;
        _http = http;
        _goals = goals;
    }

    public async Task<UserDto?> RegisterAsync(SharedRegisterDto dto, CancellationToken ct = default)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email, ct))
            throw new InvalidOperationException("Email already exists");

        var user = User.Create(
            Guid.NewGuid().ToString(),
            dto.Email,
            BCrypt.Net.BCrypt.HashPassword(dto.Password),
            dto.Name,
            dto.Age,
            dto.Gender == FireFit.Shared.Enums.Gender.Male,
            dto.Height,
            dto.StartingWeight,
            dto.TargetWeight,
            dto.WeightGoal.ToDomain(),
            dto.ActivityLevel.ToDomain(),
            dto.DietaryPreferences.Select(x => x.ToDomain()).ToList(),
            dto.WorkoutTypes.Select(x => x.ToDomain()).ToList(),
            profilePicturePath: string.Empty,
            fitnessExperience: dto.FitnessExperience.ToDomain()
        );

        _db.Users.Add(user);

        var progress = UserProgress.Create(user.UserId, dto.CurrentWeight, dto.CurrentWeight);
        _db.UserProgress.Add(progress);

        var preferences = UserPreferences.Create(user.UserId, dto.DietaryPreferences.Select(x => x.ToDomain()).ToList());
        _db.UserPreferences.Add(preferences);

        await _db.SaveChangesAsync(ct);
        return await MapUserAsync(user, ct);
    }

    public async Task<UserDto?> LoginAsync(SharedLoginDto dto, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email, ct);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        _http.HttpContext!.Response.Cookies.Append("userId", user.UserId, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        return await MapUserAsync(user, ct);
    }

    public Task LogoutAsync(CancellationToken ct = default)
    {
        _http.HttpContext!.Response.Cookies.Delete("userId");
        return Task.CompletedTask;
    }

    public async Task<UserDto?> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var userId = _http.HttpContext!.Request.Cookies["userId"];
        if (string.IsNullOrEmpty(userId)) return null;
        var user = await _db.Users.Include(u => u.CalorieLogs).FirstOrDefaultAsync(u => u.UserId == userId, ct);
        if (user == null) return null;
        return await MapUserAsync(user, ct);
    }

    private async Task<UserDto> MapUserAsync(User user, CancellationToken ct)
    {
        var dto = user.ToShared();
        var active = await _goals.GetActiveGoalAsync(user.UserId);
        if (active != null)
        {
            dto = dto with {
                DailyCalorieGoal = (int)active.NutritionalGoal.Calories,
                ProteinGoal = (int)active.NutritionalGoal.Proteins,
                CarbsGoal = (int)active.NutritionalGoal.Carbs,
                FatsGoal = (int)active.NutritionalGoal.Fats
            };
        }
        return dto;
    }

}
