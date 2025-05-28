using FireFitBlazor.Domain.Interfaces;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Services;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FireFitBlazor.Infrastructure.Contexts
{
    public class FoodLogContext : IFoodLogContext
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _loggingService;

        public FoodLogContext(ApplicationDbContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }

        public async Task<FoodLog> CreateFoodLogAsync(FoodLog foodLog)
        {
            try
            {
                foodLog.FoodLogId = Guid.NewGuid();
                foodLog.Timestamp = DateTime.UtcNow;

                _context.FoodLogs.Add(foodLog);
                await _context.SaveChangesAsync();

                _loggingService.LogInformation($"Created food log for user {foodLog.UserId}: {foodLog.FoodName}");
                return foodLog;
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Failed to create food log for user {foodLog.UserId}");
                throw;
            }
        }

        public async Task<IEnumerable<FoodLog>> GetUserFoodLogsAsync(string userId)
        {
            try
            {
                var logs = await _context.FoodLogs
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.Timestamp)
                    .ToListAsync();

                _loggingService.LogInformation($"Retrieved {logs.Count} food logs for user {userId}");
                return logs;
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Failed to retrieve food logs for user {userId}");
                throw;
            }
        }

        public async Task<bool> DeleteFoodLogAsync(Guid foodLogId)
        {
            try
            {
                var foodLog = await _context.FoodLogs.FindAsync(foodLogId);
                if (foodLog == null)
                {
                    _loggingService.LogWarning($"Attempted to delete non-existent food log: {foodLogId}");
                    return false;
                }

                _context.FoodLogs.Remove(foodLog);
                await _context.SaveChangesAsync();

                _loggingService.LogInformation($"Deleted food log {foodLogId} for user {foodLog.UserId}");
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Failed to delete food log {foodLogId}");
                throw;
            }
        }

        public async Task<IEnumerable<FoodLog>> SearchFoodAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    _loggingService.LogInformation("Empty search query received");
                    return Enumerable.Empty<FoodLog>();
                }

                var results = await _context.FoodLogs
                    .Where(f => f.FoodName.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(f => f.Timestamp)
                    .Take(10)
                    .ToListAsync();

                _loggingService.LogInformation($"Found {results.Count} food items matching query: {query}");
                return results;
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Failed to search food with query: {query}");
                throw;
            }
        }
    }
} 