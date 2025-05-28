using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetBodyMeasurementContext
    {
        Task<BodyMeasurement> Execute(int id);
        Task<BodyMeasurement> ExecuteLatestByUser(int userId);
    }
