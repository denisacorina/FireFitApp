using Microsoft.Extensions.Logging;
using System;

namespace FireFitBlazor.Domain.Services
{
    public interface ILoggingService
    {
        void LogError(Exception ex, string message);
        void LogWarning(string message);
        void LogInformation(string message);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex, string message)
        {
            _logger.LogError(ex, message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
    }
} 