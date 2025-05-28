using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.Models;
using global::FireFitBlazor.Domain.Models;
using global::FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FireFitBlazor.Domain.Contexts;


public class UserProgressContext : IUserProgressContext
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserProgressContext> _logger;

    public UserProgressContext(
        ApplicationDbContext context,
        ILogger<UserProgressContext> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserProgress> GetUserProgressAsync(string userId)
    {
        try
        {
            return await _context.UserProgress
                .Include(up => up.Measurements)
                .Include(up => up.WorkoutSessions)
                .Include(up => up.Achievements)
                .FirstOrDefaultAsync(up => up.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user progress for user {UserId}", userId);
            throw;
        }
    }

    public async Task<UserProgress> CreateProgressAsync(
        string userId,
        decimal startingWeight,
        decimal? startingBodyFat = null)
    {
        try
        {
            var progress = UserProgress.Create(
                userId,
                startingWeight,
                startingWeight,
                startingBodyFat,
                startingBodyFat
            );

            await _context.UserProgress.AddAsync(progress);
            await _context.SaveChangesAsync();

            return progress;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user progress for user {UserId}", userId);
            throw;
        }
    }

    public async Task UpdateWeightAsync(string userId, decimal newWeight, string? notes = null)
    {
        try
        {
            var progress = await GetUserProgressAsync(userId);
            if (progress == null)
                throw new ArgumentException("User progress not found");

            progress.UpdateWeight(newWeight, notes);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating weight for user {UserId}", userId);
            throw;
        }
    }

    public async Task AddMeasurementAsync(string userId, BodyMeasurement measurement)
    {
        try
        {
            var progress = await GetUserProgressAsync(userId);
            if (progress == null)
                throw new ArgumentException("User progress not found");

            progress.AddMeasurement(measurement);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding measurement for user {UserId}", userId);
            throw;
        }
    }

    public async Task AddWorkoutSessionAsync(string userId, WorkoutSession session)
    {
        try
        {
            var progress = await GetUserProgressAsync(userId);
            if (progress == null)
                throw new ArgumentException("User progress not found");

            progress.AddWorkoutSession(session);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding workout session for user {UserId}", userId);
            throw;
        }
    }
}

