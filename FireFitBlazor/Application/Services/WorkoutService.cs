using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Application.DTOs;

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
                .OrderByDescending(w => w.StartTime)
                .ToListAsync();
        }

        public async Task<List<WorkoutSession>> GetWorkoutsByDateRange(string userId, DateTime startDate, DateTime endDate)
        {
            return await _context.WorkoutSessions
                .Where(w => w.UserId == userId && w.StartTime >= startDate && w.EndTime <= endDate)
                .OrderByDescending(w => w.StartTime)
                .ToListAsync();
        }

        public async Task<WorkoutSession> GetWorkoutById(int id)
        {
            return await _context.WorkoutSessions.FindAsync(id);
        }

        public async Task<WorkoutSession> AddWorkout(WorkoutSessionDto workout)
        {
            if (workout == null)
                throw new ArgumentNullException(nameof(workout), "Workout cannot be null.");

            if (string.IsNullOrEmpty(workout.UserId))
                throw new ArgumentException("UserId cannot be null or empty.", nameof(workout.UserId));

            var newWorkout = WorkoutSession.Create(
                workout.UserId,
                workout.WorkoutType,
                workout.StartTime,
                workout.StartTime.AddMinutes(workout.DurationMinutes),
                workout.DurationMinutes,
                workout.CaloriesBurned,
                workout.IntensityLevel,
                workout.Notes);

            _context.WorkoutSessions.Add(newWorkout);
            await _context.SaveChangesAsync();
            return newWorkout;
        }

        public async Task<WorkoutSession> UpdateWorkout(WorkoutSessionDto workout)
        {
            var existingWorkout = await _context.WorkoutSessions.FindAsync(workout.SessionId);
            if (existingWorkout == null)
                throw new KeyNotFoundException($"Workout with ID {workout.SessionId} not found.");

            var updatedWorkout = existingWorkout.Update(
                workout.WorkoutType,
                workout.StartTime,
                workout.EndTime,
                workout.DurationMinutes,
                workout.CaloriesBurned,
                workout.IntensityLevel,
                workout.Notes,
                true,
                workout.Sets,
                workout.Reps);

            await _context.SaveChangesAsync();
            return updatedWorkout;
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
                .Where(w => w.UserId == userId && w.StartTime >= startDate && w.EndTime <= endDate)
                .SumAsync(w => w.CaloriesBurned);
        }

        public async Task<Dictionary<string, int>> GetWorkoutDistribution(string userId, DateTime startDate, DateTime endDate)
        {
            var workouts = await _context.WorkoutSessions
                .Where(w => w.UserId == userId && w.StartTime >= startDate && w.EndTime <= endDate)
                .ToListAsync();

            return workouts
                .GroupBy(w => w.WorkoutType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());
        }
    }
} 