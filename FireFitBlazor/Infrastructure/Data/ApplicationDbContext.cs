using FireFitBlazor.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FireFitBlazor.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FoodLog> FoodLogs { get; set; }
        public DbSet<BarcodeProduct> BarcodeProducts { get; set; }

        public DbSet<UserProgress> UserProgress { get; set; }

        public DbSet<BodyMeasurement> BodyMeasurements { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<CalorieLog> CalorieLogs { get; set; }

        public DbSet<Goal> Goals { get; set; }
        public DbSet<ExerciseLog> ExerciseLogs { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }

        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<ARPortionControl> ARPortionControl { get; set; }
        public DbSet<CalorieLog> CalorieLog { get; set; }
        public DbSet<ExerciseLog> ExerciseLog { get; set; }
   
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<IngredientRecognition> IngredientRecognition { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<WorkoutPreference> WorkoutPreferences { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .OwnsOne(f => f.WeightGoal, ni =>
                {
                    ni.Property(p => p.ChangeType).HasColumnName("WeightChangeType");
                    ni.Property(p => p.TargetWeight).HasColumnName("TargetWeight");
                });

            modelBuilder.Entity<FoodLog>()
                .OwnsOne(f => f.NutritionalInfo, ni =>
                {
                    ni.Property(p => p.Calories).HasColumnName("Calories");
                    ni.Property(p => p.Proteins).HasColumnName("Proteins");
                    ni.Property(p => p.Carbs).HasColumnName("Carbs");
                    ni.Property(p => p.Fats).HasColumnName("Fats");
                });

            modelBuilder.Entity<Recipe>()
                .OwnsOne(f => f.Nutrition, ni =>
                {
                    ni.Property(p => p.Calories).HasColumnName("Calories");
                    ni.Property(p => p.Proteins).HasColumnName("Proteins");
                    ni.Property(p => p.Carbs).HasColumnName("Carbs");
                    ni.Property(p => p.Fats).HasColumnName("Fats");
                });

            modelBuilder.Entity<BarcodeProduct>()
                .OwnsOne(f => f.Nutrition, ni =>
                {
                    ni.Property(p => p.Calories).HasColumnName("Calories");
                    ni.Property(p => p.Proteins).HasColumnName("Proteins");
                    ni.Property(p => p.Carbs).HasColumnName("Carbs");
                    ni.Property(p => p.Fats).HasColumnName("Fats");
                });

            modelBuilder.Entity<Goal>()
                .OwnsOne(f => f.NutritionalGoal, ni =>
                {
                    ni.Property(p => p.Calories).HasColumnName("Calories");
                    ni.Property(p => p.Proteins).HasColumnName("Proteins");
                    ni.Property(p => p.Carbs).HasColumnName("Carbs");
                    ni.Property(p => p.Fats).HasColumnName("Fats");
                });

            modelBuilder.Entity<Ingredient>()
                .OwnsOne(f => f.Nutrition, ni =>
                {
                    ni.Property(p => p.Calories).HasColumnName("Calories");
                    ni.Property(p => p.Proteins).HasColumnName("Proteins");
                    ni.Property(p => p.Carbs).HasColumnName("Carbs");
                    ni.Property(p => p.Fats).HasColumnName("Fats");
                });
        }
    }
} 