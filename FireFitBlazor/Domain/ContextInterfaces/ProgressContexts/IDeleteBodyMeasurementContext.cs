public interface IDeleteBodyMeasurementContext
{
    Task<bool> DeleteBodyMeasurementAsync(Guid measurementId);
}