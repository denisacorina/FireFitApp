using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.Models;
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

    public async Task<Result<UserProgress?>> GetUserProgressAsync(string userId)
    {
        try
        {
            var progress = await _context.UserProgress
          .Include(up => up.Measurements)
          .Include(up => up.WorkoutSessions)
          .Include(up => up.Achievements)
          .FirstOrDefaultAsync(up => up.UserId == userId);

           return Result<UserProgress?>.Success(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user progress for user {UserId}", userId);
            return Result<UserProgress?>.Failure("An error occurred while retrieving user progress.");
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
            var result = await GetUserProgressAsync(userId);

            if (!result.IsSuccess || result.Value == null)
            {
                throw new ArgumentException("User progress not found");
            }

            var progress = result.Value;

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
            var result = await GetUserProgressAsync(userId);

            if (!result.IsSuccess || result.Value == null)
            {
                throw new ArgumentException("User progress not found");
            }

            var progress = result.Value;

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
            var result = await GetUserProgressAsync(userId);

            if (!result.IsSuccess || result.Value == null)
            {
                throw new ArgumentException("User progress not found");
            }

            var progress = result.Value;
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

