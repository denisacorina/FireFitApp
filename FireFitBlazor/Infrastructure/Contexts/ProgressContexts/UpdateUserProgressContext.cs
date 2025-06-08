using FireFitBlazor.Application.DTOs;
using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Contexts.ProgressContexts
{
    public class UpdateUserProgressContext : IUpdateUserProgressContext
    {
        private readonly IProgressGateway _progressGateway;
        private readonly IAddBodyMeasurementContext _addBodyMeasurementContext;
        private readonly IGetBodyMeasurementsContext _getBodyMeasurementContext;
        private readonly IGetLatestBodyMeasurementContext _getLatestBodyMeasurementContext;
        private readonly IGetUserProgressContext _getUserProgressContext;
        private readonly IDeleteBodyMeasurementContext _deleteBodyMeasurementContext;

        public UpdateUserProgressContext(IProgressGateway progressGateway, IAddBodyMeasurementContext addBodyMeasurementContext, IGetBodyMeasurementsContext getBodyMeasurementContext, IGetLatestBodyMeasurementContext getLatestBodyMeasurementContext, IGetUserProgressContext getUserProgressContext,
             IDeleteBodyMeasurementContext deleteBodyMeasurementContext)
        {
            _addBodyMeasurementContext = addBodyMeasurementContext;
            _getBodyMeasurementContext = getBodyMeasurementContext;
            _getLatestBodyMeasurementContext = getLatestBodyMeasurementContext;
            _getUserProgressContext = getUserProgressContext;
            _deleteBodyMeasurementContext = deleteBodyMeasurementContext;
            _progressGateway = progressGateway;
        }

        public async Task<Result<UserProgress>> UpdateUserProgressAsync(UserProgress progress)
        {
            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            if (progress.UserId == String.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(progress));

            if (progress.CurrentWeight <= 0)
                throw new ArgumentException("Current weight must be greater than 0", nameof(progress));

            if (progress.CurrentBodyFatPercentage.HasValue && 
                (progress.CurrentBodyFatPercentage < 0 || progress.CurrentBodyFatPercentage > 100))
                throw new ArgumentException("Body fat percentage must be between 0 and 100", nameof(progress));
  
            var updatedProgress = progress.Update(progress.CurrentWeight,
                                                  progress.CurrentBodyFatPercentage,
                                                  progress.Notes,
                                                  progress.Measurements,
                                                  progress.WorkoutSessions,
                                                  progress.Achievements);

            return await _progressGateway.UpdateUserProgressAsync(updatedProgress);
        }

        public async Task<UserProgress> GetUserProgressAsync(string userId)
        {
            var progress = await GetUserProgressAsync(userId);
            return progress ?? null;
        }

        public async Task<BodyMeasurement> AddBodyMeasurementAsync(BodyMeasurement measurementDto)
        {
            var measurement = BodyMeasurement.Create(
                measurementDto.UserId,
                measurementDto.Weight,
                measurementDto.BodyFatPercentage,
                measurementDto.Chest,
                measurementDto.Waist,
                measurementDto.Hips,
                measurementDto.LeftArm,
                measurementDto.RightArm,
                measurementDto.LeftThigh,
                measurementDto.RightThigh,
                measurementDto.LeftCalf,
                measurementDto.RightCalf,
                measurementDto.Notes);

            var createdMeasurement = await _addBodyMeasurementContext.AddBodyMeasurementAsync(measurement);

            if (createdMeasurement == null)
                throw new InvalidOperationException("Failed to create body measurement");

            var progress = await _getUserProgressContext.GetUserProgressAsync(measurement.UserId);
            if (progress == null)
            {
                throw new InvalidOperationException("User progress not found.");
            }

            var updatedProgress = progress.AddMeasurement(createdMeasurement);

            if (updatedProgress == null)
                throw new InvalidOperationException("Failed to update user progress with new measurement");

            await UpdateUserProgressAsync(updatedProgress);

            return createdMeasurement;
        }

        public async Task<IEnumerable<BodyMeasurement>> GetBodyMeasurementsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var measurements = await _getBodyMeasurementContext.GetBodyMeasurementsAsync(userId, startDate, endDate);
            return measurements;
        }

        public async Task<BodyMeasurement> GetLatestBodyMeasurementAsync(string userId)
        {
            var measurement = await _getLatestBodyMeasurementContext.GetLatestBodyMeasurementAsync(userId);
            return measurement ?? null;
        }

        public async Task<bool> DeleteBodyMeasurementAsync(Guid measurementId, string userId)
        {
            var deleted = await _deleteBodyMeasurementContext.DeleteBodyMeasurementAsync(measurementId);

            if (!deleted)
                return false;

            var progress = await _getUserProgressContext.GetUserProgressAsync(userId);
            if (progress == null)
                throw new InvalidOperationException("User progress not found.");

            var updatedMeasurements = progress.Measurements
                .Where(m => m.MeasurementId != measurementId)
                .ToList();

            var updatedProgress = progress.Update(measurements: updatedMeasurements);

            await UpdateUserProgressAsync(updatedProgress);

            return true;
        }
    }
}
