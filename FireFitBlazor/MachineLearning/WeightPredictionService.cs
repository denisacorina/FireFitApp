using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Tensorflow.Keras.Layers;
using Tensorflow.NumPy;
using Tensorflow.Operations.Initializers;
using Tensorflow.Keras.Models;
using Tensorflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;
using static Tensorflow.Binding;
using Tensorflow.Keras.Engine;
using FireFitBlazor.Domain.ValueObjects;
using static WeightPredictionService;
using FireFitBlazor.Application.Services;


public interface IWeightPredictionService
{
    Task<WeightPredictionWithAnalysis> PredictWeight28Days(string userId);
}

public class WeightPredictionService
{
    private const int SEQUENCE_LENGTH = 7;
    private const int FEATURES = 3;
    private const int PREDICTION_DAYS = 28;
    private readonly ApplicationDbContext _context;
    private readonly IFoodLogService _foodLogService;
    private decimal totalCaloriesGoal = 1800;
    public WeightPredictionService(ApplicationDbContext context, IFoodLogService foodLogService)
    {
        _context = context;
        _foodLogService = foodLogService;
    }


    public class WeightData
    {
        public DateTime Date { get; set; }
        public float Weight { get; set; }
        public float Calories { get; set; }
        public float Activity { get; set; }
    }


    public async Task<WeightPredictionWithAnalysis> PredictWeight28Days(string userId)
    {
        ////// For testing purposes, use dummy data
        //var userData = CreateTestUserData();

        var userData = await GetUserDailyData(userId);

        if (userData.Length < SEQUENCE_LENGTH)
            throw new InvalidOperationException("Not enough data for prediction.");

        IModel model;
        var (trainingData, targets) = PrepareTrainingData();

        string savePath = "saved_models/weight_prediction_model_tensorflowww";

        tf.enable_eager_execution();
        if (Directory.Exists(savePath))
        {
            model = tf.keras.models.load_model(savePath);
            model.compile(optimizer: "adam", loss: "mse", metrics: new[] { "mae" });
            model.load_weights("saved_models/weightsss");
        }
        else
        {
            model = BuildModel();
            model.fit(trainingData, targets, epochs: 100, batch_size: 32, validation_split: 0.2f);
            model.save(savePath, save_format: "tf");
            model.save_weights("saved_models/weightsss");
        }

        if (model == null)
        {
            throw new InvalidOperationException("Failed to load trained model.");
        }

        // Prepare user data for prediction
        var userDataArray = ConvertUserDataToArray(userData);
        var userSequence = np.expand_dims(userDataArray, 0);
        for (int i = 0; i < 50; i++)
        { 
        // Make prediction
        var prediction1 = model.predict(userSequence);

            var predictionValue = prediction1.numpy().ToArray<float>()[0];
            predictionValue = predictionValue * 100f;
            Console.WriteLine(predictionValue);
        }

        var prediction = model.predict(userSequence);
        var currentWeight = userData.Last().Weight;
        var predictedWeight = CalculatePredictedWeight(prediction, currentWeight);
        // Analyze behavior and generate recommendations
        var behaviorAnalysis = await AnalyzeBehavior(userData, userId);
        var riskFactors = IdentifyRiskFactors(userData, behaviorAnalysis);
        var recommendations = GenerateRecommendations(userData, (decimal)predictedWeight, behaviorAnalysis);


        //userData = await GetUserDailyData(userId);


        //// Prepare user data for prediction
        // userDataArray = ConvertUserDataToArray(userData);
        // userSequence = np.expand_dims(userDataArray, 0);
        //for (int i = 0; i < 50; i++)
        //{
        //    // Make prediction
        //    var prediction1 = model.predict(userSequence);

        //    var predictionValue = prediction1.numpy().ToArray<float>()[0];
        //    predictionValue = predictionValue * 100f;
        //    Console.WriteLine(predictionValue);
        //}

        // prediction = model.predict(userSequence);
        // currentWeight = userData.Last().Weight;
        // predictedWeight = CalculatePredictedWeight(prediction, currentWeight);
        //// Analyze behavior and generate recommendations
        // behaviorAnalysis = await AnalyzeBehavior(userData, userId);
        // riskFactors = IdentifyRiskFactors(userData, behaviorAnalysis);
        // recommendations = GenerateRecommendations(userData, (decimal)predictedWeight, behaviorAnalysis);

        return new WeightPredictionWithAnalysis
        {
            CurrentWeight = (decimal)currentWeight,
            PredictedWeight = (decimal)predictedWeight,
            PredictionDate = DateTime.Today.AddDays(PREDICTION_DAYS),
            BehaviorAnalysis = behaviorAnalysis,
            RiskFactors = riskFactors,
            Recommendations = recommendations
        };
    }

    //private float CalculatePredictedWeight(Tensor prediction, float currentWeight)
    //{
    //    var predictionValue = prediction.numpy().ToArray<float>()[0];

    //    // Apply realistic constraints
    //    var maxWeightLoss = currentWeight * 0.10f; // Max 10% weight loss in 28 days
    //    var maxWeightGain = currentWeight * 0.05f; // Max 5% weight gain in 28 days

    //    var weightChange = predictionValue - currentWeight;
    //    weightChange = Math.Clamp(weightChange, -maxWeightLoss, maxWeightGain);

    //    return currentWeight + weightChange;
    //}

    private IModel BuildModel()
    {
        var inputs = tf.keras.Input(shape: new Shape(SEQUENCE_LENGTH, FEATURES));

        //var x = tf.keras.layers.LSTM(units: 32, return_sequences: true).Apply(inputs);
        //x = tf.keras.layers.LSTM(units: 16).Apply(x);
        //x = tf.keras.layers.Dense(units: 8, activation: "relu").Apply(x);
     

        var x = tf.keras.layers.LSTM(32, return_sequences: true).Apply(inputs);
        x = tf.keras.layers.Dropout(0.2f).Apply(x);
        x = tf.keras.layers.LSTM(16).Apply(x);
        x = tf.keras.layers.Dense(8, activation: "relu").Apply(x);
        x = tf.keras.layers.Dropout(0.2f).Apply(x);
   var outputs = tf.keras.layers.Dense(units: 1).Apply(x);
        var model = tf.keras.Model(inputs, outputs);
        model.compile(
            optimizer: "adam",
            loss: "mse",
            metrics: new[] { "mae" });

        return model;
    }

    private (NDArray features, NDArray targets) PrepareTrainingData()
    {
        var random = new Random(42);
        const int sampleCount = 1000;
        var features = new float[sampleCount, SEQUENCE_LENGTH, FEATURES];
        var targets = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float weight = 55f + (float)random.NextDouble() * 45f; // 55–100 kg
            float height = 160f + (float)random.NextDouble() * 20f; // 160–180 cm
            int age = 20 + random.Next(35);                         // 20–55 ani
            bool isFemale = random.NextDouble() < 0.5;
            string gender = isFemale ? "F" : "M";

            float activityMultiplier = (float)(1.2 + random.NextDouble() * 1.2); // 1.2–2.4
            float currentWeight = weight;

            // Aleatoriu alegem un scenariu: 0=deficit, 1=mentinere, 2=surplus
            int goalScenario = random.Next(3);

            for (int day = 0; day < SEQUENCE_LENGTH; day++)
            {
                float bmr = EstimateBMR(currentWeight, height, age, gender);
                float maintenanceCalories = bmr * activityMultiplier;

                float calories = goalScenario switch
                {
                    0 => maintenanceCalories - 500 + (float)(random.NextDouble() * 100 - 50), // -400 to -600 kcal
                    1 => maintenanceCalories + (float)(random.NextDouble() * 100 - 50),       // +/-50 kcal
                    2 => maintenanceCalories + 300 + (float)(random.NextDouble() * 100),      // +300–400 kcal
                    _ => maintenanceCalories
                };

                float activityLevel = (float)(1.0 + random.NextDouble() * 3.5); // 1.0–4.5

                // Greutatea variază în funcție de surplus/deficit
                float netCalories = calories - maintenanceCalories;
                float dailyWeightChange = netCalories / 7700f;
                currentWeight += dailyWeightChange;

                // Normalizare
                features[i, day, 0] = currentWeight / 100f;
                features[i, day, 1] = calories / 4000f;
                features[i, day, 2] = activityLevel / 5f;
            }

            // Ținta: greutatea finală normalizată
            targets[i] = currentWeight / 100f;
        }

        return (np.array(features), np.array(targets));
    }


    private float EstimateBMR(float weightKg, float heightCm, int age, string gender)
    {
        if (gender == "F")
            return (10 * weightKg) + (6.25f * heightCm) - (5 * age) - 161;
        else
            return (10 * weightKg) + (6.25f * heightCm) - (5 * age) + 5;
    }

    private NDArray ConvertUserDataToArray(DailyData[] userData)
    {
        //// Normalize the data
        //var maxWeight = userData.Max(d => d.Weight);
        //var maxCalories = userData.Max(d => d.CaloriesConsumed);
        //var maxActivity = userData.Max(d => d.ActivityLevel);

        var result = new float[SEQUENCE_LENGTH, FEATURES];
        for (int i = 0; i < SEQUENCE_LENGTH; i++)
        {
            result[i, 0] = userData[i].Weight / 100f;
            result[i, 1] = userData[i].CaloriesConsumed / 4000f;
            result[i, 2] = userData[i].ActivityLevel / 5f;
        }

        return np.array(result);
    }

    public class DailyData
    {
        public DateTime Date { get; set; }
        public float Weight { get; set; }
        public int CaloriesConsumed { get; set; }
        public float ActivityLevel { get; set; }
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
        public bool IsUndereating { get; set; }
        public bool IsAlignedWithGoal { get; set; }
        public WeightTrend WeightTrend { get; set; }
        public CalorieTrend CalorieTrend { get; set; }
        public ActivityMetrics ActivityConsistency { get; set; }

        public string PrimaryIssue
        {
            get
            {

                if (!IsAlignedWithGoal) return "Goal Misalignment";
                if (IsPlateauing) return "Weight Plateau";
                if (!ActivityConsistency.IsAdequate) return "Insufficient Activity";
                if (IsOvereating) return "Overeating";
                if (IsUndereating) return "Undereating";

                return "On Track";
            }
        }

    }

    public class RiskFactor
    {
        public RiskType Type { get; set; }
        public string Description { get; set; }
        public string Impact { get; set; }
        public string[] Suggestions { get; set; }
    }


    private async Task<BehaviorAnalysis> AnalyzeBehavior(DailyData[] userData, string userId)
    {
        var weightTrend = CalculateWeightTrend(userData);
        var calorieTrend = CalculateCalorieTrend(userData);
        var activityConsistency = CalculateActivityConsistency(userData);

        return new BehaviorAnalysis
        {
            IsPlateauing = CheckForPlateau(userData),
            IsOvereating = await CheckForOvereating(userData, userId),
            IsUndereating = await CheckForUndereating(userData, userId),
            IsAlignedWithGoal = CheckGoalAlignment(userData, weightTrend),
            WeightTrend = weightTrend,
            CalorieTrend = calorieTrend,
            ActivityConsistency = activityConsistency
        };
    }

    private bool CheckForPlateau(DailyData[] userData)
    {
        var weightChanges = new List<float>();
        for (int i = 1; i < userData.Length; i++)
        {
            weightChanges.Add(userData[i].Weight - userData[i - 1].Weight);
        }

        // Plateau if weight change is minimal for 3+ days
        return weightChanges.Count(c => Math.Abs(c) < 0.1f) >= 3;
    }

    private async Task<bool> CheckForOvereating(DailyData[] userData, string userId)
    {
        var avgCalories = userData.Average(d => d.CaloriesConsumed);

        var targetCalories = await _foodLogService.GetDailyGoalCalories(userId);
        return avgCalories > targetCalories;
    }

    private async Task<bool> CheckForUndereating(DailyData[] userData, string userId)
    {
        var avgCalories = userData.Average(d => d.CaloriesConsumed);

        var targetCalories = await _foodLogService.GetDailyGoalCalories(userId);
        return avgCalories < (targetCalories * 0.85);
    }


    private WeightTrend CalculateWeightTrend(DailyData[] userData)
    {
        float startWeight = userData.First().Weight;
        float endWeight = userData.Last().Weight;
        float delta = endWeight - startWeight;

        float weeklyChange = (delta / SEQUENCE_LENGTH) * 7;

        return weeklyChange switch
        {
            var x when x < -1.0f => WeightTrend.LosingFast,
            var x when x < -0.5f => WeightTrend.LosingHealthy,
            var x when x < -0.2f => WeightTrend.LosingSlow,
            var x when x > 0.5f => WeightTrend.GainingFast,
            var x when x > 0.2f => WeightTrend.Gaining,
            _ => WeightTrend.Maintaining
        };
    }


    private CalorieTrend CalculateCalorieTrend(DailyData[] userData)
    {
        var avgCalories = userData.Average(d => d.CaloriesConsumed);
        var calorieVariability = CalculateCalorieVariability(userData);
        var bmr = EstimateBasalMetabolicRate(userData.Last().Weight);

        return new CalorieTrend
        {
            AverageCalories = (float)avgCalories,
            DailyVariability = calorieVariability,
            DeficitOrSurplus = (float)(avgCalories - bmr),
            IsConsistent = calorieVariability < 300
        };
    }

    private ActivityMetrics CalculateActivityConsistency(DailyData[] userData)
    {
        var activities = userData.Select(d => d.ActivityLevel).ToList();
        var avgActivity = activities.Average();
        var stdDev = CalculateStandardDeviation(activities);

        return new ActivityMetrics
        {
            AverageLevel = avgActivity,
            Consistency = 1 - (stdDev / avgActivity),
            DaysActive = activities.Count(a => a >= 2.5f),
            TrendDirection = activities.Last() > avgActivity ? ActivityTrend.Increasing : ActivityTrend.Decreasing
        };
    }

    private float EstimateBasalMetabolicRate(float weight, float heightCm = 170, int age = 30, string gender = "F")
    {
        // Using Mifflin-St Jeor Equation
        float bmr;
        if (gender.ToUpper() == "F")
        {
            bmr = (10 * weight) + (6.25f * heightCm) - (5 * age) - 161;
        }
        else
        {
            bmr = (10 * weight) + (6.25f * heightCm) - (5 * age) + 5;
        }
        return bmr;
    }

    private float CalculateCalorieVariability(DailyData[] userData)
    {
        var calories = userData.Select(d => d.CaloriesConsumed).ToList();
        return CalculateStandardDeviation(calories.Select(c => (float)c).ToList());
    }

    private bool CheckGoalAlignment(DailyData[] userData, WeightTrend weightTrend)
    {
        var userGoal = GetUserWeightGoal(userData[0].Weight);

        return weightTrend switch
        {
            WeightTrend.LosingFast or WeightTrend.LosingHealthy when userGoal == WeightChangeType.Lose => true,
            WeightTrend.Maintaining when userGoal == WeightChangeType.Maintain => true,
            WeightTrend.Gaining when userGoal == WeightChangeType.Gain => true,
            _ => false
        };
    }

    private WeightChangeType GetUserWeightGoal(float currentWeight)
    {
        var bmi = CalculateBMI(currentWeight, 170);
        return bmi switch
        {
            var x when x > 25 => WeightChangeType.Lose,
            var x when x < 18.5 => WeightChangeType.Gain,
            _ => WeightChangeType.Maintain
        };
    }

    private float CalculateBMI(float weightKg, float heightCm)
    {
        var heightM = heightCm / 100;
        return weightKg / (heightM * heightM);
    }
    private float CalculateStandardDeviation(List<float> values)
    {
        var avg = values.Average();
        var sumOfSquaresOfDifferences = values.Select(val => (val - avg) * (val - avg)).Sum();
        return (float)Math.Sqrt(sumOfSquaresOfDifferences / (values.Count - 1));
    }

    private float CalculatePredictedWeight(Tensor prediction, float currentWeight)
    {
        var predictionValue = prediction.numpy().ToArray<float>()[0];
        predictionValue = predictionValue * 100f;

        var weightLimits = CalculateMaxSafeWeightChange(currentWeight, PREDICTION_DAYS);

        var weightChange = predictionValue - currentWeight;
        weightChange = Math.Clamp(weightChange, -weightLimits.MaxLoss, weightLimits.MaxGain);

        return currentWeight + weightChange;
    }



    private List<RiskFactor> IdentifyRiskFactors(DailyData[] userData, BehaviorAnalysis analysis)
    {
        var risks = new List<RiskFactor>();

        if (analysis.WeightTrend == WeightTrend.LosingFast)
        {
            risks.Add(new RiskFactor
            {
                Type = RiskType.RapidWeightLoss,
                Description = "Weight loss may be too rapid",
                Impact = "Could lead to muscle loss and nutritional deficiencies",
                Suggestions = new[]
                {
                    "Increase caloric intake slightly",
                    "Ensure adequate protein intake",
                    "Consider consulting a healthcare provider"
                }
            });
        }

        if (analysis.ActivityConsistency.Consistency < 0.7f)
        {
            risks.Add(new RiskFactor
            {
                Type = RiskType.InconsistentActivity,
                Description = "Activity levels are inconsistent",
                Impact = "May slow progress towards weight goals",
                Suggestions = new[]
                {
                    "Create a regular exercise schedule",
                    "Set daily activity minimums",
                    "Track activity levels more closely"
                }
            });
        }

        if (!analysis.CalorieTrend.IsConsistent)
        {
            risks.Add(new RiskFactor
            {
                Type = RiskType.InconsistentCalories,
                Description = "Large daily calorie fluctuations",
                Impact = "May affect metabolism and hunger levels",
                Suggestions = new[]
                {
                    "Plan meals in advance",
                    "Maintain consistent meal times",
                    "Track calories more accurately"
                }
            });
        }

        return risks;
    }
    public WeightChangeLimit CalculateMaxSafeWeightChange(float currentWeight, int timeframeDays)
    {
        return new WeightChangeLimit
        {
            MaxLoss = 5.0f,  
            MaxGain = 2.5f  
        };
    }

    public class WeightChangeLimit
    {
        public float MaxLoss { get; set; }
        public float MaxGain { get; set; }
    }

    private List<string> GenerateRecommendations(
        DailyData[] userData,
        decimal predictedWeight,
        BehaviorAnalysis analysis)
    {
        var recommendations = new List<string>();

        switch (analysis.WeightTrend)
        {
            case WeightTrend.LosingFast:
                recommendations.Add("⚠️ You are losing weight too quickly. Consider slowing down for long-term sustainability.");
                recommendations.Add("Increase your caloric intake slightly to reduce the risk of muscle loss.");
                recommendations.Add("Ensure you're getting enough protein and micronutrients.");
                break;

            case WeightTrend.LosingHealthy:
                recommendations.Add("✅ You're losing weight at a healthy pace. Keep up the good work!");
                recommendations.Add("Stay consistent with your eating and exercise routine.");
                break;

            case WeightTrend.LosingSlow:
                recommendations.Add("ℹ️ Your weight loss is slow. If intentional, this is okay. Otherwise, consider reducing calories slightly or increasing activity.");
                break;

            case WeightTrend.Maintaining:
                if (!analysis.IsAlignedWithGoal)
                {
                    recommendations.Add("⚠️ Your weight is stable, but it doesn't match your goal. Adjust your intake or activity to align with your target.");
                }
                else
                {
                    recommendations.Add("✅ You're maintaining your weight as planned. Keep your current habits steady.");
                }
                break;

            case WeightTrend.Gaining:
                recommendations.Add("ℹ️ You're slowly gaining weight. If your goal is to build mass, this is acceptable. Otherwise, monitor your calories.");
                break;

            case WeightTrend.GainingFast:
                recommendations.Add("⚠️ You're gaining weight too quickly. Consider lowering your caloric intake or increasing activity.");
                recommendations.Add("Watch out for high-calorie snacks or hidden sources of excess energy.");
                break;
        }

        if (analysis.CalorieTrend.DeficitOrSurplus > 500)
        {
            recommendations.Add("Your calorie surplus may be too high. Consider reducing portion sizes.");
        }

        if (analysis.IsOvereating)
        {
            recommendations.Add("Your average calorie intake is too high. Reduce portion sizes or limit extra snacks.");
        }

        if (analysis.IsUndereating || analysis.CalorieTrend.DeficitOrSurplus < -1000)
        {
            recommendations.Add("Your calorie intake is too low. Increase your intake to avoid nutrient deficiencies and muscle loss.");
        }

        if (analysis.ActivityConsistency.AverageLevel < 2.0f)
        {
            recommendations.Add("Increase daily activity level gradually");
            recommendations.Add("Try to incorporate more movement throughout your day");
        }
        else if (analysis.ActivityConsistency.Consistency < 0.7f)
        {
            recommendations.Add("Work on maintaining more consistent activity levels throughout the week");
        }

        return recommendations;
    }

    public async Task<DailyData[]> GetUserDailyData(string userId)
    {
        // Get all weight logs for the last 60 days
        var endDate = DateTime.Today;
        var startDate = endDate.AddDays(-60);
        
        var weightLogs = await _context.BodyMeasurements
            .Where(w => w.UserId == userId && w.MeasurementDate.Date >= startDate && w.MeasurementDate.Date <= endDate)
            .OrderByDescending(w => w.MeasurementDate)
            .ToListAsync();

        var foodLogs = await _context.FoodLogs
            .Where(f => f.UserId == userId && f.Timestamp.Date >= startDate && f.Timestamp.Date <= endDate)
            .Select(f => new
            {
                Date = f.Timestamp.Date,
                Calories = f.NutritionalInfo.Calories
            })
            .ToListAsync();

        var exerciseLogs = await _context.ExerciseLogs
            .Where(e => e.UserId == userId && e.Timestamp.Date >= startDate && e.Timestamp.Date <= endDate)
            .Select(e => new
            {
                Date = e.Timestamp.Date,
                Duration = e.DurationMinutes
            })
            .ToListAsync();

        // Group and organize the data
        var calorieLogs = foodLogs
            .GroupBy(f => f.Date)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Calories));

        var activities = exerciseLogs
            .GroupBy(e => e.Date)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Duration / 30f));

        // Find continuous data starting from today (or yesterday if today has no logs)
        var continuousData = new List<DailyData>();
        var currentDate = endDate;
        var lastWeight = weightLogs.FirstOrDefault()?.Weight ?? 0;

        // If no logs for today, start from yesterday
        if (!calorieLogs.ContainsKey(currentDate))
        {
            currentDate = currentDate.AddDays(-1);
        }

        // Collect continuous data
        while (currentDate >= startDate)
        {
            // Check if we have both weight and calorie data for this day
            var hasWeight = weightLogs.Any(w => w.MeasurementDate.Date == currentDate);
            var hasCalories = calorieLogs.ContainsKey(currentDate);

            if (hasWeight)
            {
                lastWeight = (decimal)weightLogs.First(w => w.MeasurementDate.Date == currentDate).Weight;
            }

            if (hasCalories)
            {
                calorieLogs.TryGetValue(currentDate, out var kcal);
                activities.TryGetValue(currentDate, out var activity);

                continuousData.Add(new DailyData
                {
                    Date = currentDate,
                    Weight = (float)lastWeight,
                    CaloriesConsumed = (int)(kcal > 0 ? kcal : 1800),
                    ActivityLevel = activity != 0 ? activity : 1f
                });
            }
            else
            {
                // If we find a gap, we need to start over from the previous day
                // But only if we haven't collected enough days yet
                if (continuousData.Count < 7)
                {
                    continuousData.Clear();
                }
                else
                {
                    // If we already have 7+ days, we can stop at the gap
                    break;
                }
            }

            currentDate = currentDate.AddDays(-1);
        }

        // Ensure we have at least 7 days of data
        if (continuousData.Count < 7)
        {
            return Array.Empty<DailyData>();
        }

        // Return the data in chronological order
        return continuousData.OrderBy(d => d.Date).ToArray();
    }
    public class CalorieTrend
    {
        public float AverageCalories { get; set; }
        public float DailyVariability { get; set; }
        public float DeficitOrSurplus { get; set; }
        public bool IsConsistent { get; set; }
    }

    public class ActivityMetrics
    {
        public float AverageLevel { get; set; }
        public float Consistency { get; set; }
        public int DaysActive { get; set; }
        public ActivityTrend TrendDirection { get; set; }
        public bool IsAdequate => AverageLevel >= 2.5f && Consistency >= 0.7f;
    }

    public enum WeightTrend
    {
        LosingFast,
        LosingHealthy,
        LosingSlow,
        Maintaining,
        Gaining,
        GainingFast
    }

    public enum ActivityTrend
    {
        Increasing,
        Stable,
        Decreasing
    }

    public enum RiskType
    {
        Overeating,
        Plateau,
        GoalMisalignment,
        RapidWeightLoss,
        InconsistentActivity,
        InconsistentCalories,
        NutritionalDeficiency
    }


    private DailyData[] CreateTestUserData()
    {

        return new DailyData[]
{
    new() { Date = DateTime.Today.AddDays(-7), Weight = 72.5f, CaloriesConsumed = 1100, ActivityLevel = 3.0f },
    new() { Date = DateTime.Today.AddDays(-6), Weight = 72.2f, CaloriesConsumed = 930, ActivityLevel = 3.2f },
    new() { Date = DateTime.Today.AddDays(-5), Weight = 72.2f, CaloriesConsumed = 900, ActivityLevel = 3.0f },
    new() { Date = DateTime.Today.AddDays(-4), Weight = 72.2f, CaloriesConsumed = 1800, ActivityLevel = 3.1f },
    new() { Date = DateTime.Today.AddDays(-3), Weight = 72.2f, CaloriesConsumed = 1100, ActivityLevel = 3.0f },
    new() { Date = DateTime.Today.AddDays(-2), Weight = 70.2f, CaloriesConsumed = 1700, ActivityLevel = 3.3f },
    new() { Date = DateTime.Today.AddDays(-1), Weight = 70.5f, CaloriesConsumed = 1100, ActivityLevel = 3.2f }
};
    }
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
public class ActivityMetrics
{
    public float AverageLevel { get; set; }
    public float Consistency { get; set; }
    public int DaysActive { get; set; }
    public ActivityTrend TrendDirection { get; set; }
    public bool IsAdequate => AverageLevel >= 2.5f && Consistency >= 0.7f;
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

public class WeightLog
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Weight { get; set; }
    public User User { get; set; }
}
