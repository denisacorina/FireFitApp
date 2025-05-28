using FireFitBlazor.Domain.Models;


    public interface IGetLatestBodyMeasurementContext
    {
        Task<BodyMeasurement> GetLatestBodyMeasurementAsync(string userId);
    }
