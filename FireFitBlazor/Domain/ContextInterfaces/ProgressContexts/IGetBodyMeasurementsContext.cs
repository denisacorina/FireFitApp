using FireFitBlazor.Domain.Models;


    public interface IGetBodyMeasurementsContext
    {
        Task<IEnumerable<BodyMeasurement>> GetBodyMeasurementsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
    }
