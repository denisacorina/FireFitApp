using FireFitBlazor.Application.DTOs;
using FireFitBlazor.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public interface IWorkoutService
{
    Task<List<WorkoutSession>> GetWorkoutsByUserId(string userId);
    Task<List<WorkoutSession>> GetWorkoutsByDateRange(string userId, DateTime startDate, DateTime endDate);
    Task<WorkoutSession> GetWorkoutById(int id);
    Task<WorkoutSession> AddWorkout(WorkoutSessionDto workout);
    Task<WorkoutSession> UpdateWorkout(WorkoutSessionDto workout);
    Task DeleteWorkout(Guid id);
    Task<int> GetTotalCaloriesBurned(string userId, DateTime startDate, DateTime endDate);
    Task<Dictionary<string, int>> GetWorkoutDistribution(string userId, DateTime startDate, DateTime endDate);
}
