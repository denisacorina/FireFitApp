using FireFitBlazor.Application.DTOs;
using FireFitBlazor.Domain.ContextInterfaces;
using FireFitBlazor.Domain.Enums;
using FireFitBlazor.Domain.Models;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Application.Services
{
    public class GoalService : IGoalService
    {
        private readonly IGoalContext _goalContext;

        public GoalService(IGoalContext goalContext)
        {
            _goalContext = goalContext;
        }

        public async Task<GoalDto> CreateGoalAsync(string userId, GoalType type, int calorieGoal, float proteinGoal, float carbGoal, float fatGoal, 
            bool intermittentFasting, int fastingWindow, DietaryPreference dietaryPreference,
            decimal? targetWeight = null, decimal? targetBodyFatPercentage = null, DateTime? targetDate = null)
        {
            var goal = Goal.Create(
                userId,
                type,
                calorieGoal,
                proteinGoal,
                carbGoal,
                fatGoal,
                intermittentFasting,
                fastingWindow,
                dietaryPreference,
                targetWeight,
                targetBodyFatPercentage,
                targetDate);

            var createdGoal = await _goalContext.CreateGoalAsync(goal);
            return MapToDto(createdGoal);
        }

        public async Task<GoalDto> UpdateGoalAsync(GoalDto goalDto)
        {
            var goal = await _goalContext.GetActiveGoalAsync(goalDto.UserId);
            if (goal == null)
                throw new InvalidOperationException("Goal not found");

            goal.Update(
                goalDto.Type,
                goalDto.CalorieGoal,
                goalDto.ProteinGoal,
                goalDto.CarbGoal,
                goalDto.FatGoal,
                goalDto.IntermittentFasting,
                goalDto.FastingWindowHours,
                goalDto.DietaryPreference,
                goalDto.TargetWeight,
                goalDto.TargetBodyFatPercentage,
                goalDto.TargetDate);

            var updatedGoal = await _goalContext.UpdateGoalAsync(goal);
            return MapToDto(updatedGoal);
        }

        public async Task<GoalDto> GetActiveGoalAsync(string userId)
        {
            var goal = await _goalContext.GetActiveGoalAsync(userId);
            return goal != null ? MapToDto(goal) : null;
        }

        public async Task<IEnumerable<GoalDto>> GetUserGoalsAsync(string userId)
        {
            var goals = await _goalContext.GetUserGoalsAsync(userId);
            return goals.Select(MapToDto);
        }

        public async Task<bool> DeleteGoalAsync(Guid goalId)
        {
            return await _goalContext.DeleteGoalAsync(goalId);
        }

        public async Task<GoalDto> MarkGoalAsCompletedAsync(Guid goalId)
        {
            var goal = await _goalContext.MarkGoalAsCompletedAsync(goalId);
            return MapToDto(goal);
        }

        public async Task<GoalDto> ReactivateGoalAsync(Guid goalId)
        {
            var goal = await _goalContext.ReactivateGoalAsync(goalId);
            return MapToDto(goal);
        }

        private static GoalDto MapToDto(Goal goal)
        {
            return new GoalDto
            {
                GoalId = goal.GoalId,
                UserId = goal.UserId,
                Type = goal.Type,
                CalorieGoal = (int)goal.NutritionalGoal.Calories,
                ProteinGoal = goal.NutritionalGoal.Proteins,
                CarbGoal = goal.NutritionalGoal.Carbs,
                FatGoal = goal.NutritionalGoal.Fats,
                IntermittentFasting = goal.IntermittentFasting,
                FastingWindowHours = goal.FastingWindowHours,
                DietaryPreference = goal.DietaryPreference,
                TargetWeight = goal.TargetWeight,
                TargetBodyFatPercentage = goal.TargetBodyFatPercentage,
                TargetDate = goal.TargetDate,
                IsActive = goal.IsActive,
                CreatedAt = goal.CreatedAt,
                CompletedAt = goal.CompletedAt
            };
        }
    }
} 