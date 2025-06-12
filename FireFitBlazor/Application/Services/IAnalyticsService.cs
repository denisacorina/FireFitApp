using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Application.Services
{
    public interface IAnalyticsService
    {
        Task<UserAnalytics> GetUserAnalytics(string userId, DateTime startDate, DateTime endDate);
    }
} 