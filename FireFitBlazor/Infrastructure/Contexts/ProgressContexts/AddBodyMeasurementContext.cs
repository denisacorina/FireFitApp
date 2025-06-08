using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;


public class AddBodyMeasurementContext : IAddBodyMeasurementContext
{
    private readonly IProgressGateway _progressGateway;

    public AddBodyMeasurementContext(IProgressGateway progressGateway)
    {
        _progressGateway = progressGateway;
    }

    public async Task<BodyMeasurement> AddBodyMeasurementAsync(BodyMeasurement measurement)
    {
        if (measurement == null)
            throw new ArgumentNullException(nameof(measurement));

        if (measurement.UserId == String.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(measurement));

        // Validate measurements
        if (measurement.Weight <= 0)
            throw new ArgumentException("Weight must be greater than 0", nameof(measurement));

        if (measurement.BodyFatPercentage.HasValue && 
            (measurement.BodyFatPercentage < 0 || measurement.BodyFatPercentage > 100))
            throw new ArgumentException("Body fat percentage must be between 0 and 100", nameof(measurement));

        // Validate body measurements
        ValidateBodyMeasurement(measurement);


        return await _progressGateway.AddBodyMeasurementAsync(measurement);
    }

    private void ValidateBodyMeasurement(BodyMeasurement measurement)
    {
        if (measurement.Chest <= 0)
            throw new ArgumentException("Chest measurement must be greater than 0", nameof(measurement));

        if (measurement.Waist <= 0)
            throw new ArgumentException("Waist measurement must be greater than 0", nameof(measurement));

        if (measurement.Hips <= 0)
            throw new ArgumentException("Hips measurement must be greater than 0", nameof(measurement));

        if (measurement.LeftArm <= 0)
            throw new ArgumentException("Left arm measurement must be greater than 0", nameof(measurement));

        if (measurement.RightArm <= 0)
            throw new ArgumentException("Right arm measurement must be greater than 0", nameof(measurement));

        if (measurement.LeftThigh <= 0)
            throw new ArgumentException("Left thigh measurement must be greater than 0", nameof(measurement));

        if (measurement.RightThigh <= 0)
            throw new ArgumentException("Right thigh measurement must be greater than 0", nameof(measurement));

        if (measurement.LeftCalf <= 0)
            throw new ArgumentException("Left calf measurement must be greater than 0", nameof(measurement));

        if (measurement.RightCalf <= 0)
            throw new ArgumentException("Right calf measurement must be greater than 0", nameof(measurement));
    }
}
