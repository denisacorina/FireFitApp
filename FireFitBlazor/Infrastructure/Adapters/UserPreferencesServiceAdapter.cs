using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using FireFit.Shared.Enums;
using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Infrastructure.Adapters;

namespace FireFitBlazor.Infrastructure.Adapters;

public class UserPreferencesServiceAdapter : IUserPreferencesService
{
    private readonly IUserPreferencesContext _ctx;
    public UserPreferencesServiceAdapter(IUserPreferencesContext ctx) { _ctx = ctx; }

    public async Task<UserPreferencesDto?> GetAsync(string userId, CancellationToken ct = default)
    {
        var p = await _ctx.GetUserPreferences(userId);
        if (p == null) return null;
        return new UserPreferencesDto
        {
            UserId = p.UserId,
            DietaryPreferences = p.DietaryPreferences.Select(d => d.ToShared()).ToList()
        };
    }

    public async Task UpdateAsync(UserPreferencesDto dto, CancellationToken ct = default)
    {
        await _ctx.UpdateUserPreferencesAsync(dto.UserId, dto.DietaryPreferences.Select(d => d.ToDomain()).ToList());
    }
}

