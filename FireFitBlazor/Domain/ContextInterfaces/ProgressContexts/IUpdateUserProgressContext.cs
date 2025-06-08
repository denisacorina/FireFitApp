using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.ContextInterfaces.ProgressContexts
{
    public interface IUpdateUserProgressContext
    {
        Task<Result<UserProgress>> UpdateUserProgressAsync(UserProgress progress);
        Task<UserProgress> GetUserProgressAsync(string userId);
        Task<BodyMeasurement> AddBodyMeasurementAsync(BodyMeasurement measurementDto);
        Task<IEnumerable<BodyMeasurement>> GetBodyMeasurementsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<BodyMeasurement> GetLatestBodyMeasurementAsync(string userId);
        Task<bool> DeleteBodyMeasurementAsync(Guid measurementId, string userId);
    }
} 