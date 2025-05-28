using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Contexts;

public class UserPreferencesContext : IUserPreferencesContext
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserPreferencesContext> _logger;

    public UserPreferencesContext(
        ApplicationDbContext context,
        ILogger<UserPreferencesContext> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task UpdateWorkoutPreferencesAsync(string userId, List<WorkoutPreference> selectedTypes)
    {
        try
        {
            // Remove existing preferences
            var existingPreferences = await _context.WorkoutPreferences
                .Where(wp => wp.UserId == userId)
                .ToListAsync();

            _context.WorkoutPreferences.RemoveRange(existingPreferences);

            // Create new preferences using the model's Create method
            var newPreferences = selectedTypes.Select(type =>
                WorkoutPreference.Create(
                    userId,
                    type.Type,
                    DayOfWeek.Monday, // Default values
                    TimeSpan.FromHours(9),
                    60,
                    5
                )).ToList();

            await _context.WorkoutPreferences.AddRangeAsync(newPreferences);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workout preferences for user {UserId}", userId);
            throw;
        }
    }
    public async Task UpdateDietaryPreferencesAsync(string userId, IEnumerable<DietaryPreference> preferences)
    {
        var user = await _context.Users.Include(i => i.Preferences)
                                         .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user != null)
        {
            if (user.Preferences == null)
            {
                user.Preferences = UserPreferences.Create(userId, new List<DietaryPreference>(), 0);
                _context.UserPreferences.Add(user.Preferences);
            }
            user.Preferences.DietaryPreferences = preferences.ToList();
            await _context.SaveChangesAsync();
        }
    }
    public async Task UpdateUserPreferencesAsync(
        string userId,
        List<DietaryPreference> dietaryPreferences,
        int dailyCalorieGoal)
    {
        try
        {
            var preferences = await _context.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (preferences == null)
            {
                preferences = UserPreferences.Create(
                    userId,
                    dietaryPreferences,
                    dailyCalorieGoal
                );
                await _context.UserPreferences.AddAsync(preferences);
            }
            else
            {
                preferences = preferences.Update(
                    dietaryPreferences,
                    dailyCalorieGoal
                );
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user preferences for user {UserId}", userId);
            throw;
        }
    }
}
