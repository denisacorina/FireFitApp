using FireFitBlazor.Application.DTOs;
using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;

namespace FireFitBlazor.Application.Services
{
    public class ProgressService : IProgressContext
    {
        private readonly IAddBodyMeasurementContext _addBodyMeasurementContext;
        private readonly IGetBodyMeasurementsContext _getBodyMeasurementContext;
        private readonly IGetLatestBodyMeasurementContext _getLatestBodyMeasurementContext;
        private readonly IGetUserProgressContext _getUserProgressContext;
        private readonly IUpdateUserProgressContext _updateUserProgressContext;
        private readonly IDeleteBodyMeasurementContext _deleteBodyMeasurementContext;

        public ProgressService(IAddBodyMeasurementContext addBodyMeasurementContext, IGetBodyMeasurementsContext getBodyMeasurementContext, IGetLatestBodyMeasurementContext getLatestBodyMeasurementContext, IGetUserProgressContext getUserProgressContext,
            IUpdateUserProgressContext updateUserProgressContext, IDeleteBodyMeasurementContext deleteBodyMeasurementContext)
        {
            _addBodyMeasurementContext = addBodyMeasurementContext;
            _getBodyMeasurementContext = getBodyMeasurementContext;
            _getLatestBodyMeasurementContext = getLatestBodyMeasurementContext;
            _getUserProgressContext = getUserProgressContext;   
            _updateUserProgressContext = updateUserProgressContext;
            _deleteBodyMeasurementContext = deleteBodyMeasurementContext;
        }

        public async Task<UserProgress> GetUserProgressAsync(string userId)
        {
            var progress = await GetUserProgressAsync(userId);
            return progress ?? null;
        }

        public async Task<UserProgress> UpdateUserProgressAsync(UserProgress progressDto)
        {
            var progress = await GetUserProgressAsync(progressDto.UserId);
            if (progress == null)
                throw new InvalidOperationException("Progress not found");

            if (progressDto.CurrentWeight != progress.CurrentWeight)
            {
                progress.UpdateWeight(progressDto.CurrentWeight, progressDto.Notes);
            }

            if (progressDto.CurrentBodyFatPercentage.HasValue &&
                progressDto.CurrentBodyFatPercentage != progress.CurrentBodyFatPercentage)
            {
                progress.UpdateBodyFat(progressDto.CurrentBodyFatPercentage.Value, progressDto.Notes);
            }

            var updatedProgress = await _updateUserProgressContext.UpdateUserProgressAsync(progress);
            return updatedProgress;
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

        public async Task<bool> DeleteBodyMeasurementAsync(Guid measurementId)
        {
            return await _deleteBodyMeasurementContext.DeleteBodyMeasurementAsync(measurementId);
        }
    }
} 