using System;
using System.Linq;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using Microsoft.Extensions.Logging;
using FireFitBlazor.Application.DTOs;

namespace FireFitBlazor.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUserProgressContext _userProgressContext;
        private readonly IFoodLogService _foodLogService;
        private readonly IWorkoutService _workoutService;
        private readonly IGoalService _goalService;
        private readonly IGetUserContext _getUserContext;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(
            IUserProgressContext userProgressContext,
            IFoodLogService foodLogService,
            IWorkoutService workoutService,
            IGoalService goalService,
            IGetUserContext getUserContext,
            ILogger<AnalyticsService> logger)
        {
            _userProgressContext = userProgressContext;
            _foodLogService = foodLogService;
            _workoutService = workoutService;
            _goalService = goalService;
            _getUserContext = getUserContext;
            _logger = logger;
        }

        public async Task<UserAnalytics> GetUserAnalytics(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Get user progress data
                var progressResult = await _userProgressContext.GetUserProgressAsync(userId);
                var progress = progressResult.Value;

                // Get food logs for the period
                var foodLogs = await _foodLogService.GetLogsForDate(userId, startDate);
                var endDateLogs = await _foodLogService.GetLogsForDate(userId, endDate);
                foodLogs.AddRange(endDateLogs);

                // Get workouts for the period
                var workouts = await _workoutService.GetWorkoutsByDateRange(userId, startDate, endDate);

                // Get active goals
                var activeGoal = await _goalService.GetActiveGoalAsync(userId);

                var analytics = new UserAnalytics
                {
                    StartingWeight = (double)progress.StartingWeight,
                    CurrentWeight = (double)progress.CurrentWeight,
                    AverageCalorieIntake = CalculateAverageCalorieIntake(foodLogs),
                    AverageCaloriesBurned = await _workoutService.GetTotalCaloriesBurned(userId, startDate, endDate),
                    AverageProtein = CalculateAverageMacro(foodLogs, "protein"),
                    AverageCarbs = CalculateAverageMacro(foodLogs, "carbs"),
                    AverageFat = CalculateAverageMacro(foodLogs, "fat"),
                    TotalWorkouts = workouts.Count,
                    TotalExerciseMinutes = workouts.Sum(w => w.DurationMinutes),
                    MostCommonExercise = GetMostCommonExercise(workouts),
                    GoalsProgress = await GetGoalsProgress(userId)
                };

                return analytics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user analytics for user {UserId}", userId);
                throw;
            }
        }

        private int CalculateAverageCalorieIntake(List<FoodLog> foodLogs)
        {
            if (!foodLogs.Any()) return 0;
            return (int)foodLogs.Average(f => f.NutritionalInfo.Calories);
        }

        private int CalculateAverageMacro(List<FoodLog> foodLogs, string macroType)
        {
            if (!foodLogs.Any()) return 0;

            return macroType.ToLower() switch
            {
                "protein" => (int)foodLogs.Average(f => f.NutritionalInfo.Proteins),
                "carbs" => (int)foodLogs.Average(f => f.NutritionalInfo.Carbs),
                "fat" => (int)foodLogs.Average(f => f.NutritionalInfo.Fats),
                _ => 0
            };
        }

        private string GetMostCommonExercise(List<WorkoutSession> workouts)
        {
            if (!workouts.Any()) return "No workouts";

            var mostCommon = workouts
                .GroupBy(w => w.WorkoutType)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;

            return mostCommon.ToString();
        }

        private async Task<List<GoalProgress>> GetGoalsProgress(string userId)
        {
            var goals = await _goalService.GetUserGoalsAsync(userId);
            return goals.Select(g => new GoalProgress
            {
                Name = g.Type.ToString(),
                Progress = CalculateGoalProgress(g)
            }).ToList();
        }

        private int CalculateGoalProgress(GoalDto goal)
        {
            var userProgress = _userProgressContext.GetUserProgressAsync(goal.UserId);
            if (userProgress == null)
            {
                _logger.LogWarning("User not found for goal {GoalId}", goal.UserId);
                return 0;
            }

            if (goal.TargetDate.HasValue && goal.TargetDate.Value < DateTime.Now)
            {
                return 100;
            }

            if (goal.TargetWeight.HasValue)
            {
                var totalChange = Math.Abs(goal.TargetWeight.Value - userProgress.Result.Value.StartingWeight);
                var currentChange = Math.Abs(userProgress.Result.Value.CurrentWeight - userProgress.Result.Value.StartingWeight);
                return (int)((currentChange / totalChange) * 100);
            }

            return 0;
        }
    }
} 