using CsvHelper;
using CsvHelper.Configuration.Attributes;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.ValueObjects;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FireFitBlazor.Application.Services
{
    public class FoodLogService : IFoodLogService
    {
        private readonly ApplicationDbContext _db;

        private readonly IHttpContextAccessor _httpContextAccessor;
        string userId { get; set; }

        public FoodLogService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<List<FoodLog>> SearchFoodsAsync(string query)
        {
            return await _db.Ingredients
                .Where(i => i.Name.Contains(query))
                .Select(i => new FoodLog(
                    i.Name,
                    i.Nutrition.Calories,
                    i.Nutrition.Proteins,
                    i.Nutrition.Carbs,
                    i.Nutrition.Fats
                ))
                .ToListAsync();
        }

        public async Task ImportIngredientsSimple(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<IngredientCsv>().ToList();

            foreach (var r in records)
            {
                float calories = (float)Math.Round((r.Data_Carbohydrate * 4) + (r.Data_Protein * 4) + (r.Data_Fat_TotalLipid * 9), 2, MidpointRounding.AwayFromZero);

                var ingredient = new Ingredient
                {
                    IngredientId = Guid.NewGuid(),
                    Name = r.Category,
                    Description = r.Description,
                    Nutrition = NutritionalInfo.Create(calories, r.Data_Protein, r.Data_Carbohydrate, r.Data_Fat_TotalLipid)
                };

                _db.Ingredients.Add(ingredient);
            }

            await _db.SaveChangesAsync();
        }


        public class IngredientCsv
        {
            [Name("Category")]
            public string Category { get; set; }

            [Name("Description")]
            public string? Description { get; set; }

            [Name("Data.Carbohydrate")]
            public float Data_Carbohydrate { get; set; }

            [Name("Data.Protein")]
            public float Data_Protein { get; set; }

            [Name("Data.Fat.Total Lipid")]
            public float Data_Fat_TotalLipid { get; set; }
        }


        public async Task<List<Ingredient>> GetAllIngredientNames()
        {
            return await _db.Ingredients
                .OrderBy(name => name)
                .ToListAsync();
        }

        //public async Task LogFoodAsync(FoodLog item)
        //{

        //    if (item.UserId == null)
        //        throw new UnauthorizedAccessException("User not logged in.");

        //    var log = FoodLog.Create(
        //        userId: item.UserId,
        //        foodName: item.FoodName,
        //        calories: (int)item.NutritionalInfo.Calories,
        //        proteins: item.NutritionalInfo.Proteins,
        //        carbs: item.NutritionalInfo.Carbs,
        //        fats: item.NutritionalInfo.Fats
        //    );

        //    _db.FoodLogs.Add(log);
        //    await _db.SaveChangesAsync();
        //}

        public async Task<List<FoodLog>> GetLogsForDate(string userId, DateTime date)
        {
            return await _db.FoodLogs
                .Where(f => f.UserId == userId && f.Timestamp.Date == date.Date)
                .ToListAsync();
        }

        public async Task<int> GetDailyGoalCalories(string userId)
        {
            var goal = await _db.Goals
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .FirstOrDefaultAsync();

            return (int)(goal?.NutritionalGoal.Calories ?? 2000);
        }

        //public async Task<List<DetectedIngredient>> DetectIngredientsFromImage(string base64)
        //{
        //    // Simulated detection (replace with real model call)
        //    return new List<DetectedIngredient>
        //    {
        //new("Spinach", 20, 30, "/images/spinach.png"),
        //new("Tomato", 70, 35, "/images/tomato.png"),
        //new("Egg", 25, 60, "/images/egg.png"),
        //new("Olive oil", 55, 75, "/images/oil.png")
        //    };
        //}


        public async Task<NutritionalSummary> GetMacrosForIngredients(List<string> ingredientNames)
        {
            var matched = await _db.Ingredients
                .Where(i => ingredientNames.Contains(i.Name))
                .ToListAsync();

            return new NutritionalSummary
            {
                TotalCalories = (int)matched.Sum(i => i.Nutrition.Calories),
                Fats = matched.Sum(i => i.Nutrition.Fats),
                Proteins = matched.Sum(i => i.Nutrition.Proteins),
                Carbs = matched.Sum(i => i.Nutrition.Carbs),
            };
        }

        public async Task<List<Ingredient>> GetIngredientDetails(List<string> names)
        {
            return await _db.Ingredients
                .Where(i => names.Contains(i.Name))
                .ToListAsync();
        } 
        
        public async Task<List<Ingredient>> GetIngredientDetailsById(List<Guid> ingredientIds)
        {
            return await _db.Ingredients
                .Where(i => ingredientIds.Contains(i.IngredientId))
                .ToListAsync();
        }


        public async Task<Ingredient> GetIngredientDetails(Guid ingredientId)
        {
            var ingredient = _db.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
            return ingredient;
        }

        public async Task SaveFoodLogAsync(FoodLog log)
        {
            _db.FoodLogs.Add(log);
            await _db.SaveChangesAsync();
        }


        public async Task SaveMealAsync(string mealName, string userId, List<DetectedIngredient> ingredients)
        {
            var meal = new Meal
            {
                MealId = Guid.NewGuid(),
                Name = mealName,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Ingredients = ingredients.Select(i => new MealIngredient
                {
                    MealIngredientId = Guid.NewGuid(),
                    IngredientName = i.Name,
                    QuantityGrams = i.QuantityGrams
                }).ToList()
            };

            //_db.Meals.Add(meal);
            //await _db.SaveChangesAsync();
        }

        public async Task DeleteLog(Guid logId)
        {

            var log = await _db.FoodLogs.FirstOrDefaultAsync(f => f.FoodLogId == logId);
            if (log != null)
            {
                _db.FoodLogs.Remove(log);
                await _db.SaveChangesAsync();
            }

        }

        public async Task<FoodLog?> GetLogById(Guid logId)
        {

            return await _db.FoodLogs.FirstOrDefaultAsync(f => f.FoodLogId == logId);
        }

        public async Task UpdateFoodLogAsync(FoodLog updatedLog)
        {
            var existing = await _db.FoodLogs.FirstOrDefaultAsync(f => f.FoodLogId == updatedLog.FoodLogId);
            if (existing != null)
            {
                existing.FoodName = updatedLog.FoodName;
                existing.MealType = updatedLog.MealType;
                existing.NutritionalInfo = updatedLog.NutritionalInfo;
                existing.Timestamp = updatedLog.Timestamp;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<FoodLog>> GetRecentByUserIdAsync(string userId, int days)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            return await _db.FoodLogs
                .Where(f => f.UserId == userId && f.Timestamp >= cutoffDate)
                .OrderByDescending(f => f.Timestamp)
                .ToListAsync();
        }

        public async Task<List<UserIngredientHistory>> GetUserIngredientHistoryAsync(string userId, int limit = 20)
        {
            return await _db.UserIngredientHistory
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.LastUsed)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<UserIngredientHistory> AddToUserHistoryAsync(string userId, Guid ingredientId, string ingredientName)
        {
            var existingHistory = await _db.UserIngredientHistory
                .FirstOrDefaultAsync(h => h.UserId == userId && h.IngredientId == ingredientId);

            if (existingHistory != null)
            {
                existingHistory = existingHistory.UpdateUsage();
                //_db.UserIngredientHistory.Update(existingHistory);
            }
            else
            {
                existingHistory = UserIngredientHistory.Create(userId, ingredientId, ingredientName);
                _db.UserIngredientHistory.Add(existingHistory);
            }

            await _db.SaveChangesAsync();
            return existingHistory;
        }

        public async Task<UserIngredientHistory> UpdateIngredientUsageAsync(Guid historyId)
        {
            var history = await _db.UserIngredientHistory.FindAsync(historyId);
            if (history == null)
                throw new KeyNotFoundException($"No history found with ID {historyId}");

            history = history.UpdateUsage();
            //_db.UserIngredientHistory.Update(history);
            await _db.SaveChangesAsync();
            return history;
        }
    }

    public class FoodItem
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Calories { get; set; }
        public decimal Fats { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fibers { get; set; }

    }

}
