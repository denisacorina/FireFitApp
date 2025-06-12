using System;
using System.Collections.Generic;

namespace FireFitBlazor.Domain.Models
{
    public class UserAnalytics
    {
        public double StartingWeight { get; set; }
        public double CurrentWeight { get; set; }
        public int AverageCalorieIntake { get; set; }
        public int AverageCaloriesBurned { get; set; }
        public int AverageProtein { get; set; }
        public int AverageCarbs { get; set; }
        public int AverageFat { get; set; }
        public int TotalWorkouts { get; set; }
        public int TotalExerciseMinutes { get; set; }
        public string MostCommonExercise { get; set; }
        public List<GoalProgress> GoalsProgress { get; set; } = new();
    }

    public class GoalProgress
    {
        public string Name { get; set; }
        public int Progress { get; set; }
    }
} 