using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IBodyMeasurementContext
    {
        Task<BodyMeasurement> Execute(Guid id);
        Task<IEnumerable<BodyMeasurement>> GetUserMeasurements(string userId);
        Task<bool> AddMeasurement(BodyMeasurement measurement);
        Task<bool> UpdateMeasurement(BodyMeasurement measurement);
        Task<bool> DeleteMeasurement(Guid id);
    }
