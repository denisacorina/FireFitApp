using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IGetListCalorieLogGateway
{
    Task<IEnumerable<CalorieLog>> GetByUserIdAsync(int userId);
    Task<IEnumerable<CalorieLog>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
}
