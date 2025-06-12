using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkoutSession>> GetWorkoutsByUserId(string userId)
        {
            return await _context.WorkoutSessions
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .ToListAsync();
        }

        public async Task<List<WorkoutSession>> GetWorkoutsByDateRange(string userId, DateTime startDate, DateTime endDate)
        {
            return await _context.WorkoutSessions
                .Where(w => w.UserId == userId && w.Date >= startDate && w.Date <= endDate)
                .OrderByDescending(w => w.Date)
                .ToListAsync();
        }

        public async Task<WorkoutSession> GetWorkoutById(int id)
        {
            return await _context.WorkoutSessions.FindAsync(id);
        }

        public async Task<WorkoutSession> AddWorkout(WorkoutSession workout)
        {
            var addedWorkout = WorkoutSession.Create(
                workout.UserId,
                workout.WorkoutType,
                workout.StartTime,
                workout.IntensityLevel,
                workout.Notes);
            _context.WorkoutSessions.Add(addedWorkout);
            await _context.SaveChangesAsync();
            return workout;
        }

        public async Task<WorkoutSession> UpdateWorkout(WorkoutSession workout)
        {
            var existingWorkout = await _context.WorkoutSessions.FindAsync(workout.SessionId);
            if (existingWorkout == null)
                throw new KeyNotFoundException($"Workout with ID {workout.SessionId} not found.");

            var updatedWorkout = existingWorkout.CompleteSession(
                workout.Exercises.ToList());

            await _context.SaveChangesAsync();
            return existingWorkout;
        }

        public async Task DeleteWorkout(Guid id)
        {
            var workout = await _context.WorkoutSessions.FindAsync(id);
            if (workout == null)
                throw new KeyNotFoundException($"Workout with ID {id} not found.");

            _context.WorkoutSessions.Remove(workout);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalCaloriesBurned(string userId, DateTime startDate, DateTime endDate)
        {
            return await _context.WorkoutSessions
                .Where(w => w.UserId == userId && w.Date >= startDate && w.Date <= endDate)
                .SumAsync(w => w.CaloriesBurned);
        }

        public async Task<Dictionary<string, int>> GetWorkoutDistribution(string userId, DateTime startDate, DateTime endDate)
        {
            var workouts = await _context.WorkoutSessions
                .Where(w => w.UserId == userId && w.Date >= startDate && w.Date <= endDate)
                .ToListAsync();

            return workouts
                .GroupBy(w => w.WorkoutType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());
        }
    }
} 