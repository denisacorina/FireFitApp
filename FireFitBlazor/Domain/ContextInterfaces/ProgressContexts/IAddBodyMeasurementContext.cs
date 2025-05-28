using FireFitBlazor.Domain.Models;

    public interface IAddBodyMeasurementContext
    {
        Task<BodyMeasurement> AddBodyMeasurementAsync(BodyMeasurement measurement);
    }

