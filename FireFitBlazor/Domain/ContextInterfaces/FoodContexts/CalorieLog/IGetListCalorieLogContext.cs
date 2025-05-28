using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetListCalorieLogContext
    {
        Task<IEnumerable<CalorieLog>> ExecuteByUser(int userId);
        Task<IEnumerable<CalorieLog>> ExecuteByUserAndDateRange(int userId, DateTime startDate, DateTime endDate);
    }
