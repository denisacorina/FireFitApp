using System.Text.Json;
using Microsoft.ML;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;
using Microsoft.ML.Data;


public interface IWeightPredictionService
{
    Task<WeightPredictionWithAnalysis> PredictWeight28Days(string userId);
}

public class WeightPredictionService : IWeightPredictionService
{
    private readonly WeightPredictionML _ml;

    public WeightPredictionService(ApplicationDbContext context)
    {
        _ml = new WeightPredictionML(context);
    }

    public async Task<WeightPredictionWithAnalysis> PredictWeight28Days(string userId)
    {
        return await _ml.PredictWeight28Days(userId);
    }
}
public class WeightPredictionML
{
    private readonly MLContext _mlContext;
    private readonly ApplicationDbContext _context;

    public WeightPredictionML(ApplicationDbContext context)
    {
        _mlContext = new MLContext(seed: 0);
        _context = context;
    }

    public async Task<WeightPredictionWithAnalysis> PredictWeight28Days(string userId)
    {

        //var mlService = new WeightPredictionML(_context);
        //await mlService.TrainModelFromJson("synthetic_weight_data.json");

      
        var user = await CollectUser(userId);
        var userprogress = await CollectUserProgress(user.UserId);
        var analysis = await AnalyzeUserBehavior(user);
        var prediction = await GeneratePrediction(user, analysis);

        return new WeightPredictionWithAnalysis
        {
            CurrentWeight = userprogress.CurrentWeight,
            PredictedWeight = prediction.PredictedWeight,
            PredictionDate = DateTime.Today.AddDays(28),
            BehaviorAnalysis = analysis,
            RiskFactors = IdentifyRiskFactors(analysis),
            Recommendations = GenerateRecommendations(analysis)
        };
    }

    private async Task<BehaviorAnalysis> AnalyzeUserBehavior(User user)
    {
        var analysis = new BehaviorAnalysis
        {
            IsPlateauing = await CheckForPlateau(user),
            IsOvereating = await CheckForOvereating(user),
            IsAlignedWithGoal = await CheckGoalAlignment(user)
        };
        return analysis;
    }

    private async Task<bool> CheckForOvereating(User user)
    {
        var tdee = await CalculateTDEE(user);
        var recentCalories = user.CalorieLogs
            .OrderByDescending(c => c.Date)
            .Take(7);

        return recentCalories.Count(c => c.ConsumedCalories > tdee) >= 5;
    }

    private async Task<bool> CheckGoalAlignment(User user)
    {
        if (user.WeightGoal.ChangeType == WeightChangeType.Lose)
        {
            var avgCalories = user.CalorieLogs.Average(c => c.ConsumedCalories);
            var tdee = await CalculateTDEE(user);
            return avgCalories < tdee;
        }
        return true;
    }

    private async Task<bool> CheckForPlateau(User user)
    {
        var userProgress = await CollectUserProgress(user.UserId);

        if (userProgress.LastMeasurementDate.HasValue &&
            (DateTime.Today - userProgress.LastMeasurementDate.Value).TotalDays <= 7)
        {
            var weightChange = Math.Abs(userProgress.CurrentWeight - userProgress.StartingWeight);
            return weightChange <= 0.5m; 
        }

        return false;
    }

    private List<RiskFactor> IdentifyRiskFactors(BehaviorAnalysis analysis)
    {
        var risks = new List<RiskFactor>();

        if (analysis.IsOvereating)
        {
            risks.Add(new RiskFactor
            {
                Type = RiskType.Overeating,
                Description = "Consistent calorie surplus detected",
                Impact = "Weight loss goal may not be achieved",
                Suggestions = new[] {
                    "Track portions more carefully",
                    "Plan meals in advance",
                    "Identify trigger foods"
                }
            });
        }

        return risks;
    }

    private List<string> GenerateRecommendations(BehaviorAnalysis analysis)
    {
        var recommendations = new List<string>();

        if (analysis.IsOvereating)
            recommendations.Add("Focus on portion control and meal planning.");

        if (!analysis.IsOvereating && analysis.IsAlignedWithGoal)
            recommendations.Add("Great job! Keep up the good work.");

        return recommendations;
    }

    private async Task<double> CalculateTDEE(User user)
    {
        var userProgress = await CollectUserProgress(user.UserId);

        decimal bmr = (user.Gender == Gender.Male)
            ? 10 * userProgress.CurrentWeight + 6.25M * user.Height - 5 * user.Age + 5
            : 10 * userProgress.CurrentWeight + 6.25M * user.Height - 5 * user.Age - 161;

        decimal activityFactor = user.ActivityLevel switch
        {
            ActivityLevel.Sedentary => 1.2M,
            ActivityLevel.Light => 1.375M,
            ActivityLevel.Moderate => 1.55M,
            ActivityLevel.Active => 1.725M,
            ActivityLevel.VeryActive => 1.9M,
            _ => 1.2M
        };

        return (double)(bmr * activityFactor);
    }

    private async Task<User> CollectUser(string userId)
    {
        var user = await _context.Users
            .Include(u => u.CalorieLogs)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            throw new Exception("User not found.");

        return user;
    }

    private async Task<UserProgress> CollectUserProgress(string userId)
    {
        var user = await _context.UserProgress
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            throw new Exception("User not found.");

        return user;
    }

    public class WeightPredictionInput
    {
        public float CurrentWeight { get; set; }
        public float AverageCalories { get; set; }
        public float TDEE { get; set; }
        public float IsOvereating { get; set; }
        public float IsAlignedWithGoal { get; set; }

        [ColumnName("Label")]
        public float PredictedWeight { get; set; } 
    }

    public class WeightPredictionOutput
    {
        public float Score { get; set; }
    }

    public async Task TrainModelFromJson(string jsonFilePath)
    {
        var jsonData = await File.ReadAllTextAsync(jsonFilePath);
        var data = JsonSerializer.Deserialize<List<WeightPredictionInput>>(jsonData);
     
        var dataView = _mlContext.Data.LoadFromEnumerable(data);

        var pipeline = _mlContext.Transforms.Concatenate("Features",
             nameof(WeightPredictionInput.CurrentWeight),
             nameof(WeightPredictionInput.AverageCalories),
             nameof(WeightPredictionInput.TDEE),
             nameof(WeightPredictionInput.IsOvereating),
             nameof(WeightPredictionInput.IsAlignedWithGoal))
         .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: "Label"));

        var dummyInput = new WeightPredictionInput
        {
            CurrentWeight = 100f,
            AverageCalories = 2400f,
            TDEE = 2200f,
            IsOvereating = 0f,
            IsAlignedWithGoal = 1f
        };

        var model = pipeline.Fit(dataView);

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<WeightPredictionInput, WeightPredictionOutput>(model);
        var prediction = predictionEngine.Predict(dummyInput);
        Console.WriteLine($"Predicted weight after 28 days: {prediction.Score:F1} kg");
        var predictions = model.Transform(dataView);
        var metrics = _mlContext.Regression.Evaluate(predictions);

        Console.WriteLine($"R2: {metrics.RSquared:F2}");
        Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:F2}");
        _mlContext.Model.Save(model, dataView.Schema, "WeightPredictionModel.zip");
    }

    private async Task<PredictionResult> GeneratePrediction(User user, BehaviorAnalysis analysis)
    {
        var userProgress = await CollectUserProgress(user.UserId);
        decimal currentWeight = userProgress.CurrentWeight;
        var input = new WeightPredictionInput
        {
            CurrentWeight = (float)currentWeight,
            AverageCalories = (float)user.CalorieLogs.Average(c => c.ConsumedCalories),
            TDEE = (float)await CalculateTDEE(user),
            IsOvereating = analysis.IsOvereating ? 1f : 0f,
            IsAlignedWithGoal = analysis.IsAlignedWithGoal ? 1f : 0f
        };

        var dummyInput = new WeightPredictionInput
        {
            CurrentWeight = 80f,
            AverageCalories = 2500f,
            TDEE = 2200f,
            IsOvereating = 1f,
            IsAlignedWithGoal = 0f
        };
        var loadedModel = _mlContext.Model.Load("WeightPredictionModel.zip", out _);
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<WeightPredictionInput, WeightPredictionOutput>(loadedModel);

        var prediction = predictionEngine.Predict(dummyInput);
        return new PredictionResult
        {
            PredictedWeight = (decimal)prediction.Score
        };
    }
}

public class PredictionResult
{
    public decimal PredictedWeight { get; set; }
}

public class WeightPredictionWithAnalysis
{
    public decimal CurrentWeight { get; set; }
    public decimal PredictedWeight { get; set; }
    public DateTime PredictionDate { get; set; }
    public BehaviorAnalysis BehaviorAnalysis { get; set; }
    public List<RiskFactor> RiskFactors { get; set; }
    public List<string> Recommendations { get; set; }
}

public class BehaviorAnalysis
{
    public bool IsPlateauing { get; set; } 
    public bool IsOvereating { get; set; }
    public bool IsAlignedWithGoal { get; set; }
    public string PrimaryIssue =>
        IsOvereating ? "Overeating" :
        !IsAlignedWithGoal ? "Goal Misalignment" : "On Track";
}

public class RiskFactor
{
    public RiskType Type { get; set; }
    public string Description { get; set; }
    public string Impact { get; set; }
    public string[] Suggestions { get; set; }
}

public enum RiskType
{
    Overeating,
    Plateau,
    GoalMisalignment
}

