using FireFit.Shared.Enums;

namespace FireFit.Shared.DTOs;

public class UserPreferencesDto
{
    public string UserId { get; set; } = string.Empty;
    public IEnumerable<DietaryPreference> DietaryPreferences { get; set; } = Array.Empty<DietaryPreference>();
}

