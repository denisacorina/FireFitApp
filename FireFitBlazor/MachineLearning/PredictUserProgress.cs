using FireFitBlazor.Domain.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using static TorchSharp.torch.utils;

namespace FireFitBlazor.MachineLearning;


using Microsoft.ML;
using Microsoft.ML.Data;
using System.Text.Json;

public class UserProgressPredictor
{
    private readonly MLContext _mlContext = new();



    // Structuri de date
    public class CustomUserFeatures
    {
        public float BMR { get; set; }
        public float CaloriesAvg { get; set; }
        public float ProteinsAvg { get; set; }
        public float CarbsAvg { get; set; }
        public float FatsAvg { get; set; }
        public float StepsAvg { get; set; }
        public float WorkoutMinutesAvg { get; set; }
        public float StartWeight { get; set; }
        public float Height { get; set; }
        public float Age { get; set; }
        public int IsMale { get; set; }
        [ColumnName("Label")]
        public float FutureWeight { get; set; } // Numai la antrenament
    }

    public class UserPeriodData
    {
        public float CaloriesAvg { get; set; }
        public float ProteinsAvg { get; set; }
        public float CarbsAvg { get; set; }
        public float FatsAvg { get; set; }
        public float StepsAvg { get; set; }
        public float WorkoutMinutesAvg { get; set; }
        public float StartWeight { get; set; }
        public float Height { get; set; }
        public float Age { get; set; }
        public bool IsMale { get; set; }
    }

    public class WeightPrediction
    {
        [ColumnName("Score")]
        public float PredictedWeight { get; set; }
    }
}

public class PredictUserProgress
{
    public PredictUserProgress() { }

    public void PredictUserProgressNow()
    {
        var mlContext = new MLContext();

        var data = new List<UserPeriodData>
{
    new UserPeriodData { CaloriesAvg = 2100, ProteinsAvg = 110, CarbsAvg = 240, FatsAvg = 70, StepsAvg = 9000, WorkoutMinutesAvg = 40, StartWeight = 85, FutureWeight = 84 },
    new UserPeriodData { CaloriesAvg = 2200, ProteinsAvg = 120, CarbsAvg = 250, FatsAvg = 75, StepsAvg = 9100, WorkoutMinutesAvg = 45, StartWeight = 84, FutureWeight = 83.9f },
    new UserPeriodData { CaloriesAvg = 2300, ProteinsAvg = 125, CarbsAvg = 260, FatsAvg = 80, StepsAvg = 9200, WorkoutMinutesAvg = 50, StartWeight = 83.9f , FutureWeight = 83.4f },
    new UserPeriodData { CaloriesAvg = 2000, ProteinsAvg = 100, CarbsAvg = 220, FatsAvg = 65, StepsAvg = 9500, WorkoutMinutesAvg = 35, StartWeight = 83.4f, FutureWeight = 82.7f },
    new UserPeriodData { CaloriesAvg = 1900, ProteinsAvg = 95, CarbsAvg = 210, FatsAvg = 60, StepsAvg = 10000, WorkoutMinutesAvg = 30, StartWeight = 82.7f, FutureWeight = 81.9f },
    new UserPeriodData { CaloriesAvg = 2000, ProteinsAvg = 105, CarbsAvg = 230, FatsAvg = 65, StepsAvg = 9800, WorkoutMinutesAvg = 40, StartWeight = 81.9f, FutureWeight = 81.5f },
    new UserPeriodData { CaloriesAvg = 2100, ProteinsAvg = 110, CarbsAvg = 240, FatsAvg = 70, StepsAvg = 9500, WorkoutMinutesAvg = 45, StartWeight = 81.5f, FutureWeight = 80.9f },
    new UserPeriodData { CaloriesAvg = 2150, ProteinsAvg = 130, CarbsAvg = 240, FatsAvg = 70, StepsAvg = 10500, WorkoutMinutesAvg = 45, StartWeight = 81.5f, FutureWeight = 80.9f },
    new UserPeriodData { CaloriesAvg = 2200, ProteinsAvg = 115, CarbsAvg = 245, FatsAvg = 72, StepsAvg = 9400, WorkoutMinutesAvg = 50, StartWeight = 80.9f, FutureWeight = 80.3f },
    new UserPeriodData { CaloriesAvg = 2300, ProteinsAvg = 120, CarbsAvg = 250, FatsAvg = 75, StepsAvg = 9300, WorkoutMinutesAvg = 55, StartWeight = 80.3f, FutureWeight = 79.8f },
    new UserPeriodData { CaloriesAvg = 2100, ProteinsAvg = 105, CarbsAvg = 235, FatsAvg = 68, StepsAvg = 9700, WorkoutMinutesAvg = 42, StartWeight = 79.8f, FutureWeight = 79.2f },
    new UserPeriodData { CaloriesAvg = 2000, ProteinsAvg = 100, CarbsAvg = 225, FatsAvg = 65, StepsAvg = 9900, WorkoutMinutesAvg = 35, StartWeight = 79.2f, FutureWeight = 78.6f },
    new UserPeriodData { CaloriesAvg = 1900, ProteinsAvg = 95, CarbsAvg = 215, FatsAvg = 60, StepsAvg = 10200, WorkoutMinutesAvg = 32, StartWeight = 78.6f, FutureWeight = 78.0f },
};



        var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Concatenate("Features",
                nameof(UserPeriodData.CaloriesAvg),
                nameof(UserPeriodData.ProteinsAvg),
                nameof(UserPeriodData.CarbsAvg),
                nameof(UserPeriodData.FatsAvg),
                nameof(UserPeriodData.StepsAvg),
                nameof(UserPeriodData.WorkoutMinutesAvg),
                nameof(UserPeriodData.StartWeight))
            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 200));

        var model = pipeline.Fit(dataView);

        // Predictie pentru un user nou!
        var newData = new UserPeriodData
        {
            CaloriesAvg = 1950,
            ProteinsAvg = 105,
            CarbsAvg = 220,
            FatsAvg = 65,
            StepsAvg = 10000,
            WorkoutMinutesAvg = 38,
            StartWeight = 78.0f
        };

        var predictionEngine = mlContext.Model.CreatePredictionEngine<UserPeriodData, WeightPrediction>(model);
        var prediction = predictionEngine.Predict(newData);

        Console.WriteLine($"🔮 Predicted weight in 7 days: {prediction.PredictedWeight:F2} kg");
    }


    public float PredictWeightForNextWeek()
    {

        var dailyData = new List<UserDailyData>
{
    new UserDailyData { Calories = 2200, Proteins = 110, Carbs = 250, Fats = 70, Steps = 9500, WorkoutMinutes = 45, CurrentWeight = 85.0f },
    new UserDailyData { Calories = 2100, Proteins = 105, Carbs = 240, Fats = 68, Steps = 9600, WorkoutMinutes = 40, CurrentWeight = 84.8f },
    new UserDailyData { Calories = 2150, Proteins = 112, Carbs = 245, Fats = 69, Steps = 9700, WorkoutMinutes = 42, CurrentWeight = 84.5f },
    new UserDailyData { Calories = 2000, Proteins = 100, Carbs = 230, Fats = 65, Steps = 9800, WorkoutMinutes = 38, CurrentWeight = 84.3f },
    new UserDailyData { Calories = 2050, Proteins = 105, Carbs = 235, Fats = 66, Steps = 9900, WorkoutMinutes = 40, CurrentWeight = 84.0f },
    new UserDailyData { Calories = 1950, Proteins = 95, Carbs = 220, Fats = 63, Steps = 10000, WorkoutMinutes = 36, CurrentWeight = 83.8f },
    new UserDailyData { Calories = 1900, Proteins = 90, Carbs = 215, Fats = 60, Steps = 10200, WorkoutMinutes = 32, CurrentWeight = 83.5f },
};

        // Date statice (ale utilizatorului)
        var userHeight = 178;  // exemplu
        var userAge = 25;
        var userBMR = 1700;  // bazat pe formula de calcul BMR
        var isMale = 1;
        var mlContext = new MLContext();

        // 1️⃣ Construiește structura UserPeriodFeatures
        var features = new UserPeriodFeatures
        {
            Day1Calories = dailyData[0].Calories,
            Day1Proteins = dailyData[0].Proteins,
            Day1Carbs = dailyData[0].Carbs,
            Day1Fats = dailyData[0].Fats,
            Day1Steps = dailyData[0].Steps,
            Day1Workout = dailyData[0].WorkoutMinutes,
            Day1Weight = dailyData[0].CurrentWeight,

            Day2Calories = dailyData[1].Calories,
            Day2Proteins = dailyData[1].Proteins,
            Day2Carbs = dailyData[1].Carbs,
            Day2Fats = dailyData[1].Fats,
            Day2Steps = dailyData[1].Steps,
            Day2Workout = dailyData[1].WorkoutMinutes,
            Day2Weight = dailyData[1].CurrentWeight,

            Day3Calories = dailyData[2].Calories,
            Day3Proteins = dailyData[2].Proteins,
            Day3Carbs = dailyData[2].Carbs,
            Day3Fats = dailyData[2].Fats,
            Day3Steps = dailyData[2].Steps,
            Day3Workout = dailyData[2].WorkoutMinutes,
            Day3Weight = dailyData[2].CurrentWeight,

            Day4Calories = dailyData[3].Calories,
            Day4Proteins = dailyData[3].Proteins,
            Day4Carbs = dailyData[3].Carbs,
            Day4Fats = dailyData[3].Fats,
            Day4Steps = dailyData[3].Steps,
            Day4Workout = dailyData[3].WorkoutMinutes,
            Day4Weight = dailyData[3].CurrentWeight,

            Day5Calories = dailyData[4].Calories,
            Day5Proteins = dailyData[4].Proteins,
            Day5Carbs = dailyData[4].Carbs,
            Day5Fats = dailyData[4].Fats,
            Day5Steps = dailyData[4].Steps,
            Day5Workout = dailyData[4].WorkoutMinutes,
            Day5Weight = dailyData[4].CurrentWeight,

            Day6Calories = dailyData[5].Calories,
            Day6Proteins = dailyData[5].Proteins,
            Day6Carbs = dailyData[5].Carbs,
            Day6Fats = dailyData[5].Fats,
            Day6Steps = dailyData[5].Steps,
            Day6Workout = dailyData[5].WorkoutMinutes,
            Day6Weight = dailyData[5].CurrentWeight,

            Day7Calories = dailyData[6].Calories,
            Day7Proteins = dailyData[6].Proteins,
            Day7Carbs = dailyData[6].Carbs,
            Day7Fats = dailyData[6].Fats,
            Day7Steps = dailyData[6].Steps,
            Day7Workout = dailyData[6].WorkoutMinutes,
            Day7Weight = dailyData[6].CurrentWeight,

            Height = userHeight,
            Age = userAge,
            BMR = userBMR,
            IsMale = isMale,
            TargetWeight = dailyData[6].CurrentWeight // Folosim greutatea curentă ca etichetă pentru predicție
        };

        // 2️⃣ Exemplu de date pentru antrenament (în realitate, ia-le din DB!)
        var trainData = new List<UserPeriodFeatures>
    {
        // Adaugă minim 30-50 mostre reale (tu le pui din DB)
        features
    };

        var dataView = mlContext.Data.LoadFromEnumerable(trainData);

        // 3️⃣ Pipeline ML
        var pipeline = mlContext.Transforms.Concatenate("Features",
            nameof(UserPeriodFeatures.Day1Calories),
            nameof(UserPeriodFeatures.Day1Proteins),
            nameof(UserPeriodFeatures.Day1Carbs),
            nameof(UserPeriodFeatures.Day1Fats),
            nameof(UserPeriodFeatures.Day1Steps),
            nameof(UserPeriodFeatures.Day1Workout),
            nameof(UserPeriodFeatures.Day1Weight),

            nameof(UserPeriodFeatures.Day2Calories),
            nameof(UserPeriodFeatures.Day2Proteins),
            nameof(UserPeriodFeatures.Day2Carbs),
            nameof(UserPeriodFeatures.Day2Fats),
            nameof(UserPeriodFeatures.Day2Steps),
            nameof(UserPeriodFeatures.Day2Workout),
            nameof(UserPeriodFeatures.Day2Weight),

            nameof(UserPeriodFeatures.Day3Calories),
            nameof(UserPeriodFeatures.Day3Proteins),
            nameof(UserPeriodFeatures.Day3Carbs),
            nameof(UserPeriodFeatures.Day3Fats),
            nameof(UserPeriodFeatures.Day3Steps),
            nameof(UserPeriodFeatures.Day3Workout),
            nameof(UserPeriodFeatures.Day3Weight),

            nameof(UserPeriodFeatures.Day4Calories),
            nameof(UserPeriodFeatures.Day4Proteins),
            nameof(UserPeriodFeatures.Day4Carbs),
            nameof(UserPeriodFeatures.Day4Fats),
            nameof(UserPeriodFeatures.Day4Steps),
            nameof(UserPeriodFeatures.Day4Workout),
            nameof(UserPeriodFeatures.Day4Weight),

            nameof(UserPeriodFeatures.Day5Calories),
            nameof(UserPeriodFeatures.Day5Proteins),
            nameof(UserPeriodFeatures.Day5Carbs),
            nameof(UserPeriodFeatures.Day5Fats),
            nameof(UserPeriodFeatures.Day5Steps),
            nameof(UserPeriodFeatures.Day5Workout),
            nameof(UserPeriodFeatures.Day5Weight),

            nameof(UserPeriodFeatures.Day6Calories),
            nameof(UserPeriodFeatures.Day6Proteins),
            nameof(UserPeriodFeatures.Day6Carbs),
            nameof(UserPeriodFeatures.Day6Fats),
            nameof(UserPeriodFeatures.Day6Steps),
            nameof(UserPeriodFeatures.Day6Workout),
            nameof(UserPeriodFeatures.Day6Weight),

            nameof(UserPeriodFeatures.Day7Calories),
            nameof(UserPeriodFeatures.Day7Proteins),
            nameof(UserPeriodFeatures.Day7Carbs),
            nameof(UserPeriodFeatures.Day7Fats),
            nameof(UserPeriodFeatures.Day7Steps),
            nameof(UserPeriodFeatures.Day7Workout),
            nameof(UserPeriodFeatures.Day7Weight),

            nameof(UserPeriodFeatures.Height),
            nameof(UserPeriodFeatures.Age),
            nameof(UserPeriodFeatures.BMR),
            nameof(UserPeriodFeatures.IsMale)
        ).Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 200));

        // 4️⃣ Antrenează
        var model = pipeline.Fit(dataView);

        // 5️⃣ Creează engine de predicție și prezice
        var predictionEngine = mlContext.Model.CreatePredictionEngine<UserPeriodFeatures, WeightPrediction>(model);
        var prediction = predictionEngine.Predict(features);

        return prediction.PredictedWeight;
    }


    public async Task TrainModelFromJson(string jsonFilePath)
    {
        // Load JSON dataset
        var jsonData = await File.ReadAllTextAsync(jsonFilePath);
        var data = JsonSerializer.Deserialize<List<WeightPredictionInput>>(jsonData);

        // Initialize MLContext
        var mlContext = new MLContext(seed: 0);

        // Load data into IDataView
        var dataView = mlContext.Data.LoadFromEnumerable(data);

        // Define data preparation & pipeline
        var pipeline = mlContext.Transforms.Concatenate("Features",
                nameof(WeightPredictionInput.CurrentWeight),
                nameof(WeightPredictionInput.AverageCalories),
                nameof(WeightPredictionInput.TDEE),
                nameof(WeightPredictionInput.IsPlateauing),
                nameof(WeightPredictionInput.IsOvereating),
                nameof(WeightPredictionInput.IsAlignedWithGoal))
            .Append(mlContext.Regression.Trainers.FastTree());

        // Train the model
        var model = pipeline.Fit(dataView);

        // Save the model
        mlContext.Model.Save(model, dataView.Schema, "WeightPredictionModel.zip");
    }
    public class UserPeriodFeatures
    {
        // Datele zilnice pentru 7 zile
        public float Day1Calories { get; set; }
        public float Day1Proteins { get; set; }
        public float Day1Carbs { get; set; }
        public float Day1Fats { get; set; }
        public float Day1Steps { get; set; }
        public float Day1Workout { get; set; }
        public float Day1Weight { get; set; }

        public float Day2Calories { get; set; }
        public float Day2Proteins { get; set; }
        public float Day2Carbs { get; set; }
        public float Day2Fats { get; set; }
        public float Day2Steps { get; set; }
        public float Day2Workout { get; set; }
        public float Day2Weight { get; set; }

        public float Day3Calories { get; set; }
        public float Day3Proteins { get; set; }
        public float Day3Carbs { get; set; }
        public float Day3Fats { get; set; }
        public float Day3Steps { get; set; }
        public float Day3Workout { get; set; }
        public float Day3Weight { get; set; }

        public float Day4Calories { get; set; }
        public float Day4Proteins { get; set; }
        public float Day4Carbs { get; set; }
        public float Day4Fats { get; set; }
        public float Day4Steps { get; set; }
        public float Day4Workout { get; set; }
        public float Day4Weight { get; set; }

        public float Day5Calories { get; set; }
        public float Day5Proteins { get; set; }
        public float Day5Carbs { get; set; }
        public float Day5Fats { get; set; }
        public float Day5Steps { get; set; }
        public float Day5Workout { get; set; }
        public float Day5Weight { get; set; }

        public float Day6Calories { get; set; }
        public float Day6Proteins { get; set; }
        public float Day6Carbs { get; set; }
        public float Day6Fats { get; set; }
        public float Day6Steps { get; set; }
        public float Day6Workout { get; set; }
        public float Day6Weight { get; set; }

        public float Day7Calories { get; set; }
        public float Day7Proteins { get; set; }
        public float Day7Carbs { get; set; }
        public float Day7Fats { get; set; }
        public float Day7Steps { get; set; }
        public float Day7Workout { get; set; }
        public float Day7Weight { get; set; }

        // Informații statice
        public float Height { get; set; }
        public float Age { get; set; }
        public float BMR { get; set; }
        public float IsMale { get; set; } // 1 sau 0

        [ColumnName("Label")]
        public float TargetWeight { get; set; }
    }
    public class WeightDiffPrediction
    {
        [ColumnName("Score")]
        public float WeightDiff { get; set; }
    }

    public class WeightPredictionInput
    {
        public float CurrentWeight { get; set; }
        public float AverageCalories { get; set; }
        public float TDEE { get; set; }
        public float IsPlateauing { get; set; }
        public float IsOvereating { get; set; }
        public float IsAlignedWithGoal { get; set; }
        public float PredictedWeight { get; set; }
    }


    public class UserDayData
    {
        public float Calories { get; set; }
        public float Proteins { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
        public float Steps { get; set; }
        public float WorkoutMinutes { get; set; }
        [ColumnName("Label")]
        public float CurrentWeight { get; set; }
    }

    public class UserDailyData
    {
        public float Calories { get; set; }
        public float Proteins { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
        public float Steps { get; set; }
        public float WorkoutMinutes { get; set; }
        public float CurrentWeight { get; set; } // Greutatea din ziua asta
        public float WeightDiff { get; set; } // Greutatea viitoare - curenta (label)
    }

    public class UserPeriodData
    {
        public float CaloriesAvg { get; set; }
        public float ProteinsAvg { get; set; }
        public float CarbsAvg { get; set; }
        public float FatsAvg { get; set; }
        public float StepsAvg { get; set; }
        public float WorkoutMinutesAvg { get; set; }
        public float StartWeight { get; set; }
        public float FutureWeight { get; set; }
    }


    public class UserProgressInput
    {
        public float AvgCalories { get; set; }
        public float AvgProteins { get; set; }
        public float AvgCarbs { get; set; }
        public float AvgFats { get; set; }
        public float AvgSteps { get; set; }
        public float AvgWorkoutMinutes { get; set; }
        public float CurrentWeight { get; set; }
        public float Height { get; set; }
        public int Age { get; set; }
        public bool IsMale { get; set; }
    }

    public class WeightPrediction
    {
        [ColumnName("Score")]
        public float PredictedWeight { get; set; }
    }
}


