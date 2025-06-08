using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public interface IProgressGateway
{
    Task<UserProgress> GetUserProgressAsync(string userId);
    Task<Result<UserProgress>> UpdateUserProgressAsync(UserProgress progress);
    Task<BodyMeasurement> AddBodyMeasurementAsync(BodyMeasurement measurement);
    Task<IEnumerable<BodyMeasurement>> GetBodyMeasurementsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<BodyMeasurement> GetLatestBodyMeasurementAsync(string userId);
    Task<bool> DeleteBodyMeasurementAsync(Guid measurementId);
}
public class ProgressGateway : IProgressGateway
{
    protected readonly ApplicationDbContext _dbContext;

    public ProgressGateway(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProgress> GetUserProgressAsync(string userId)
    {
        return await _dbContext.UserProgress
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<Result<UserProgress>> UpdateUserProgressAsync(UserProgress progress)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == progress.UserId);
        if (existingUser == null)
            throw new InvalidOperationException("User not found.");

        // Overwrite properties safely
        _dbContext.Entry(existingUser).CurrentValues.SetValues(progress);
      
        var result = await _dbContext.SaveChangesAsync();
        if (result <= 0)
        {
            return Result<UserProgress>.Failure("Failed to update user progress.");
        }
        return Result<UserProgress>.Success(progress);
    }

    public async Task<BodyMeasurement> AddBodyMeasurementAsync(BodyMeasurement measurement)
    {
        _dbContext.BodyMeasurements.Add(measurement);
        await _dbContext.SaveChangesAsync();
        return measurement;
    }

    public async Task<IEnumerable<BodyMeasurement>> GetBodyMeasurementsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbContext.BodyMeasurements
            .Where(m => m.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(m => m.MeasurementDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(m => m.MeasurementDate <= endDate.Value);

        return await query
            .OrderByDescending(m => m.MeasurementDate)
            .ToListAsync();
    }

    public async Task<BodyMeasurement> GetLatestBodyMeasurementAsync(string userId)
    {
        return await _dbContext.BodyMeasurements
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.MeasurementDate)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteBodyMeasurementAsync(Guid measurementId)
    {
        var measurement = await _dbContext.BodyMeasurements.FindAsync(measurementId);
        if (measurement == null)
            return false;

        _dbContext.BodyMeasurements.Remove(measurement);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
