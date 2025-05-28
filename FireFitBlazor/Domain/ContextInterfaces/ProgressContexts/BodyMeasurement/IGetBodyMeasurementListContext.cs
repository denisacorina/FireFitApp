using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetBodyMeasurementListContext
    {
        Task<IEnumerable<BodyMeasurement>> ExecuteByUser(int userId);
        Task<IEnumerable<BodyMeasurement>> ExecuteByDateRange(int userId, DateTime startDate, DateTime endDate);
    }
