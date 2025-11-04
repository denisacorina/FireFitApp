using Microsoft.ML;
using Microsoft.ML.Data;

using static BioTaggedSentence;
using Newtonsoft.Json;

using Tensorflow.NumPy;
using Tensorflow;

using static Tensorflow.Binding;


using Tensorflow.Operations;
using OneOf.Types;
using System.Text.Json;
using Tensorflow.Checkpoint;
using System.Runtime.ConstrainedExecution;
using RecipeRecommendation;
using System.Text.RegularExpressions;
using FireFitBlazor.Application.Services;
using IntentClassification;
using static TorchSharp.torch;
using Tensorflow.Keras.Layers;
using Tensorflow.Keras.ArgsDefinition;
using FireFitBlazor.Domain.ValueObjects;
using CsvHelper.Configuration.Attributes;
using System.Diagnostics;
using Tensorflow.Keras.Engine;
using Tensorflow.Operations.Initializers;



namespace RecipeRecommendation
{
    class RecipeRecommendationGen
    {
        public static void TrainNERModel1()
        {
            //var ingredients = File.ReadAllLines("food.csv")
            // .Skip(1)
            // .Select(line =>
            // {
            //     var parts = line.Split(',');

            //     decimal TryParseDecimal(string input)
            //     {
            //         return decimal.TryParse(input.Trim('"'), out var value) ? value : 0;
            //     }

            //     return new IngredientNutrition
            //     {
            //         NameRaw = parts[1].Trim('"').ToLower(),            // Clean name properly
            //         Calories = TryParseDecimal(parts[11]),          // Data.Kilocalories
            //         Carbs = TryParseDecimal(parts[7]),              // Data.Carbohydrate
            //         Protein = TryParseDecimal(parts[17]),           // Data.Protein
            //         Fat = TryParseDecimal(parts[27]),               // Data.Fat.Total Lipid         // Data.Fiber
            //     };
            // }).ToList();

            //var recipes = System.Text.Json.JsonSerializer.Deserialize<List<RecipeRec>>(File.ReadAllText("recipes_combined_all_with_diet.json"));
            //  BioTagGenerator.GenerateBIO("nlp_recipe_intents_15000.json", "food.csv", "bio_annotated_dataset_old_new.json");
            // TrainModelForTextClassification();

            //PredictUserIntent();

            // var dataset = LoadBIOAnnotatedDataset("bio_annotated_dataset.json");
            //  var (X, y, word2idx, tag2idx) = PrepareData(dataset);
            //
            // var model = BuildNERModel(word2idx.Count, tag2idx.Count);

            TrainModel();
            TestPrediction();


            //var ner = new NERPredictor("./ner_model", "./vocab.json");
            //var service = new RecipeGeneratorService();

            ////        var originalRecipe = new Recipe
            ////        {
            ////            Name = "Veggie Rice Bowl",
            ////            Ingredients = new List<IngredientEntry>
            ////{
            ////    new() { Name = "rice", Quantity = 150, Unit = "g" },
            ////    new() { Name = "broccoli", Quantity = 100, Unit = "g" },
            ////    new() { Name = "olive oil", Quantity = 10, Unit = "g" }
            ////}
            ////        };

            //string json = File.ReadAllText("C:\\Users\\z004umbe\\Downloads\\updated_recipes_with_nutrition_v2.json");

            //// Deserialize the JSON into a list of Recipe objects
            //var recipesJson = JsonConvert.DeserializeObject<List<RecipeJson>>(json);

            //List<RecipeJson> allRecipes = recipesJson; // Assuming this is a method that retrieves all recipes
            //Console.WriteLine("Enter your recipe request (e.g., 'vegan recipe under 400 kcal'):");
            //string userInput = Console.ReadLine();

            //// Handle the request
            //var receivedRecipe = service.HandleUserRequest(userInput, allRecipes);
            ////// User request: Find vegan recipes under 400 kcal
            ////string dietaryPreference = "Vegan";
            ////decimal maxCalories = 400;

            //// Step 1: Filter the recipes by calorie limit and dietary preference
            ////var filteredRecipes = service.FilterRecipes(allRecipes, maxCalories, dietaryPreference);

            ////// Step 2: Adjust the recipes if they exceed the calorie limit
            ////foreach (var recipe in filteredRecipes)
            ////{
            ////    if (recipe.TotalCalories > maxCalories)
            ////    {
            ////        recipe = service.AdjustRecipeToFitCalorieLimit(recipe, maxCalories);
            ////    }
            ////}

            //Console.Write("\nWould you like to change something to the recipe? ");
            //var userInputReplace = Console.ReadLine();

            //var updated = service.GenerateUpdatedRecipe(userInputReplace, receivedRecipe);

            //Console.WriteLine($"\n✅ Updated Recipe: {updated.Title}");
            //foreach (var ing in updated.Ingredients)
            //    Console.WriteLine($"- {ing.Ingredient}: {ing.Quantity}{ing.Unit}");

            //Console.WriteLine($"\n🔥 Calories: {updated.Calories} kcal");
            //Console.WriteLine($"💪 Protein:  {updated.Protein} g");
            //Console.WriteLine($"🍞 Carbs:    {updated.Carbs} g");
            //Console.WriteLine($"🧈 Fat:      {updated.Fat} g");




            ////var cache = new NutritionCache();

            ////IngredientNutrition GetNutrition(string name) =>
            ////    cache.GetOrAdd(name, () =>
            ////        ingredients.FirstOrDefault(i => i.Name == name.ToLower()));

            ////Console.WriteLine($"Updated total calories: {totalCalories} kcal");

            ////decimal totalCalories = recipe.Ingredients.Sum(ingredient =>
            ////{
            ////    var normalizedGrams = UnitNormalizer.NormalizeToGrams(ingredient.Quantity, ingredient.Unit);
            ////    var nut = GetNutrition(ingredient.Name);
            ////    return normalizedGrams / 100 * nut?.Calories ?? 0;
            ////});
            //var (sess, inputTensor, logitsTensor, word2idx, idx2tag) = LoadNERModel();
            //var result = PredictNER("replace butter with avocado", word2idx, idx2tag, sess, inputTensor, logitsTensor);
            //Console.WriteLine($"Old: {result.OldIngredient}, New: {result.NewIngredient}");
        }


        public class TokenInput
        {
            [LoadColumn(0)]
            public string PrevToken { get; set; }

            [LoadColumn(1)]
            public string Token { get; set; }

            [LoadColumn(2)]
            public string NextToken { get; set; }

            [LoadColumn(3)]
            public string Label { get; set; }
        }

        public class TokenSample
        {
            public string Token { get; set; }
            public string Tag { get; set; }
        }

        public class SentenceSample
        {
            public List<TokenSample> Tokens { get; set; }
        }

        public class NerInput
        {
            public string Sentence { get; set; }      // e.g., "add coconut milk to the list"
            public string[] Tokens { get; set; }      // [ "add", "coconut", "milk", "to", ... ]
            public string[] Tags { get; set; }        // [ "O", "B-NEW", "I-NEW", "O", ... ]
        }


      public class NerTokenRow
{
    public int SentenceId { get; set; }
    public string Token { get; set; }
    public string Tag { get; set; }
}

        public static void TrainML()
        {
            var mlContext = new MLContext();
            var modelPath = "bio_ner_model3.zip";
            var data = mlContext.Data.LoadFromTextFile<TokenInput>(
                "bio_dataset_flat3.tsv", separatorChar: '\t', hasHeader: true);

            var pipeline = mlContext.Transforms.Text.FeaturizeText("PrevFeats", nameof(TokenInput.PrevToken))
                .Append(mlContext.Transforms.Text.FeaturizeText("CurrFeats", nameof(TokenInput.Token)))
                .Append(mlContext.Transforms.Text.FeaturizeText("NextFeats", nameof(TokenInput.NextToken)))
                .Append(mlContext.Transforms.Concatenate("Features", "PrevFeats", "CurrFeats", "NextFeats"))
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Label"))
                .Append(mlContext.MulticlassClassification.Trainers.LightGbm("Label", "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            var sw = Stopwatch.StartNew();
            var model = pipeline.Fit(data);

            sw.Stop();
            Console.WriteLine($"⏱ ML.NET LightGBM Training Time: {sw.Elapsed.TotalSeconds:0.###} sec");
            var predictions = model.Transform(data);
            //var metrics = mlContext.MulticlassClassification.Evaluate(predictions, labelColumnName: "Label", predictedLabelColumnName: "PredictedLabel");

            //var confusion = metrics.ConfusionMatrix;
            //Console.WriteLine("\n📊 Evaluation Metrics:");
            //Console.WriteLine($"  MicroAccuracy:    {metrics.MicroAccuracy:0.###}");
            //Console.WriteLine($"  MacroAccuracy:    {metrics.MacroAccuracy:0.###}");
            //Console.WriteLine($"  LogLoss:          {metrics.LogLoss:0.###}");
            //Console.WriteLine($"  LogLossReduction: {metrics.LogLossReduction:0.###}");

            //Console.WriteLine("\n📉 Per-Class Metrics (Confusion Matrix):");


            //for (int i = 0; i < confusion.NumberOfClasses; i++)
            //{
            //    Console.WriteLine($"Class {i}:");
            //    Console.WriteLine($"  Precision: {confusion.PerClassPrecision[i]:0.###}");
            //    Console.WriteLine($"  Recall:    {confusion.PerClassRecall[i]:0.###}");
            //}

            //var sentence = "replace butter with cheese";

            //var testSentence = "replace sugar with stevia";
            //var tokens = testSentence.Split(' ');

            //var testInputs = new List<TokenInput>();
            //for (int i = 0; i < tokens.Length; i++)
            //{
            //    var prev = i > 0 ? tokens[i - 1] : "<START>";
            //    var curr = tokens[i];
            //    var next = i < tokens.Length - 1 ? tokens[i + 1] : "<END>";

            //    testInputs.Add(new TokenInput
            //    {
            //        PrevToken = prev,
            //        Token = curr,
            //        NextToken = next
            //    });
            //}

            var predEngine = mlContext.Model.CreatePredictionEngine<TokenInput, NerPrediction>(model);

            //Console.WriteLine("Predicted tags:");
            //foreach (var input in testInputs)
            //{
            //    var prediction = predEngine.Predict(input);
            //    Console.WriteLine($"{input.Token,-10} => {prediction.PredictedLabel}");
            //}

            Console.WriteLine("\n📊 Evaluating ML.NET on test examples...");

            Dictionary<string, (int TP, int FP, int FN)> mlMetrics = new();

            var testExamples = new List<NerTestSample>

{
                  new() {
        Tokens = new[] { "replace", "sugar", "with", "stevia" },
        Labels = new[] { "O", "B-OLD", "O", "B-NEW" }
    },
    new() {
        Tokens = new[] { "substitute", "butter", "with", "margarine" },
        Labels = new[] { "O", "B-OLD", "O", "B-NEW" }
    },
    new() {
        Tokens = new[] { "swap", "milk", "for", "almond", "milk" },
        Labels = new[] { "O", "B-OLD", "O", "B-NEW", "I-NEW" }
    },
       new() {
        Tokens = new[] { "I", "want", "to", "get", "rid", "of", "milk" },
        Labels = new[] { "O", "O", "O", "O", "O", "O", "B-OLD" }
    },
           new() {
        Tokens = new[] { "include", "honey" },
        Labels = new[] { "O", "B-NEW" }
    },
    new() {
        Tokens = new[] { "replace", "cream", "cheese", "with", "quark" },
        Labels = new[] { "O", "B-OLD", "I-OLD", "O", "B-NEW" }
    },
    new() {
        Tokens = new[] { "can", "I", "use", "tempeh", "instead", "of", "bacon" },
        Labels = new[] { "O", "O", "O", "B-NEW", "O", "O", "B-OLD" }
    },
    new() {
        Tokens = new[] { "try", "ghee", "instead", "of", "olive", "oil" },
        Labels = new[] { "O", "B-NEW", "O", "O", "B-OLD", "I-OLD" }
    },
    new() {
        Tokens = new[] { "get", "rid", "of", "white", "sugar" },
        Labels = new[] { "O", "O", "O", "B-OLD", "I-OLD" }
    },
    new() {
        Tokens = new[] { "add", "chia", "seeds" },
        Labels = new[] { "O", "B-NEW", "I-NEW" }
    },
    new() {
        Tokens = new[] { "use", "coconut", "nectar", "as", "a", "sweetener" },
        Labels = new[] { "O", "B-NEW", "I-NEW", "O", "O", "O" }
    },
    new() {
        Tokens = new[] { "remove", "whey", "protein" },
        Labels = new[] { "O", "B-OLD", "I-OLD" }
    },
    new() {
        Tokens = new[] { "swap", "paneer", "for", "tofu" },
        Labels = new[] { "O", "B-OLD", "O", "B-NEW" }
    },
    new() {
        Tokens = new[] { "I", "want", "to", "avoid", "mayonnaise" },
        Labels = new[] { "O", "O", "O", "O", "B-OLD" }
    },
    new() {
        Tokens = new[] { "can", "you", "replace", "cashew", "milk", "with", "soy", "milk" },
        Labels = new[] { "O", "O", "O", "B-OLD", "I-OLD", "O", "B-NEW", "I-NEW" }
    }
};


            foreach (var sample in testExamples)
            {
                for (int i = 0; i < sample.Tokens.Length; i++)
                {
                    var prev = i > 0 ? sample.Tokens[i - 1] : "<START>";
                    var curr = sample.Tokens[i];
                    var next = i < sample.Tokens.Length - 1 ? sample.Tokens[i + 1] : "<END>";

                    var input = new TokenInput { PrevToken = prev, Token = curr, NextToken = next };
                    var prediction = predEngine.Predict(input).PredictedLabel;
                    var trueLabel = sample.Labels[i];

                    if (!mlMetrics.ContainsKey(trueLabel)) mlMetrics[trueLabel] = (0, 0, 0);
                    if (!mlMetrics.ContainsKey(prediction)) mlMetrics[prediction] = (0, 0, 0);

                    if (trueLabel == prediction)
                        mlMetrics[trueLabel] = (mlMetrics[trueLabel].TP + 1, mlMetrics[trueLabel].FP, mlMetrics[trueLabel].FN);
                    else
                    {
                        mlMetrics[prediction] = (mlMetrics[prediction].TP, mlMetrics[prediction].FP + 1, mlMetrics[prediction].FN);
                        mlMetrics[trueLabel] = (mlMetrics[trueLabel].TP, mlMetrics[trueLabel].FP, mlMetrics[trueLabel].FN + 1);
                    }
                }
            }

            foreach (var tag in mlMetrics.Keys)
            {
                var (tp, fp, fn) = mlMetrics[tag];
                double precision = tp / (double)(tp + fp + 1e-6);
                double recall = tp / (double)(tp + fn + 1e-6);
                double f1 = 2 * precision * recall / (precision + recall + 1e-6);
                Console.WriteLine($"{tag,-6} => P: {precision:0.###}, R: {recall:0.###}, F1: {f1:0.###}");
            }

            mlContext.Model.Save(model, data.Schema, modelPath);
            Console.WriteLine("Model saved to: " + modelPath);
        }

        public static void PredictFromSavedModel(string sentence)
        {
            var mlContext = new MLContext();
            var modelPath = "bio_ner_model3.zip";

            ITransformer loadedModel = mlContext.Model.Load(modelPath, out _);
            var predictor = mlContext.Model.CreatePredictionEngine<TokenInput, NerPrediction>(loadedModel);

            var tokens = sentence.Split(' ');
            for (int i = 0; i < tokens.Length; i++)
            {
                var prev = i > 0 ? tokens[i - 1] : "<START>";
                var curr = tokens[i];
                var next = i < tokens.Length - 1 ? tokens[i + 1] : "<END>";

                var input = new TokenInput
                {
                    PrevToken = prev,
                    Token = curr,
                    NextToken = next
                };

                var prediction = predictor.Predict(input);
                Console.WriteLine($"{curr} => {prediction.PredictedLabel}");
            }
        }

        public static void TrainNERModel()
        {
            //var ingredients = File.ReadAllLines("food.csv")
            // .Skip(1)
            // .Select(line =>
            // {
            //     var parts = line.Split(',');

            //     decimal TryParseDecimal(string input)
            //     {
            //         return decimal.TryParse(input.Trim('"'), out var value) ? value : 0;
            //     }

            //     return new IngredientNutrition
            //     {
            //         NameRaw = parts[1].Trim('"').ToLower(),            // Clean name properly
            //         Calories = TryParseDecimal(parts[11]),          // Data.Kilocalories
            //         Carbs = TryParseDecimal(parts[7]),              // Data.Carbohydrate
            //         Protein = TryParseDecimal(parts[17]),           // Data.Protein
            //         Fat = TryParseDecimal(parts[27]),               // Data.Fat.Total Lipid         // Data.Fiber
            //     };
            // }).ToList();

            //var recipes = System.Text.Json.JsonSerializer.Deserialize<List<RecipeRec>>(File.ReadAllText("recipes_combined_all_with_diet.json"));
            //  BioTagGenerator.GenerateBIO("nlp_recipe_intents_15000.json", "food.csv", "bio_annotated_dataset_old_new.json");
            // TrainModelForTextClassification();

            //PredictUserIntent();

            // var dataset = LoadBIOAnnotatedDataset("bio_annotated_dataset.json");
            //  var (X, y, word2idx, tag2idx) = PrepareData(dataset);
            //
            // var model = BuildNERModel(word2idx.Count, tag2idx.Count);

            //TrainModel();
            TrainBiLSTMModel();
            TestPrediction();
        }
        public static (string? OldIngredient, string? NewIngredient) ExtractEntitiesFromNer(string sentence)
        {
            var mlContext = new MLContext();
            TrainML();
            var modelPath = "bio_ner_model3.zip";
            ITransformer loadedModel = mlContext.Model.Load(modelPath, out _);
            var predictor = mlContext.Model.CreatePredictionEngine<TokenInput, NerPrediction>(loadedModel);

            var tokens = sentence.Split(' ');
            string? oldIngredient = null, newIngredient = null;
            List<string> oldTokens = new(), newTokens = new();

            for (int i = 0; i < tokens.Length; i++)
            {
                var prev = i > 0 ? tokens[i - 1] : "<START>";
                var curr = tokens[i];
                var next = i < tokens.Length - 1 ? tokens[i + 1] : "<END>";

                var input = new TokenInput { PrevToken = prev, Token = curr, NextToken = next };
                var prediction = predictor.Predict(input);

                switch (prediction.PredictedLabel)
                {
                    case "B-OLD":
                    case "I-OLD":
                        oldTokens.Add(curr);
                        break;
                    case "B-NEW":
                    case "I-NEW":
                        newTokens.Add(curr);
                        break;
                }
            }

            if (oldTokens.Count > 0)
                oldIngredient = string.Join(" ", oldTokens);
            if (newTokens.Count > 0)
                newIngredient = string.Join(" ", newTokens);


            return (oldIngredient?.ToLowerInvariant(), newIngredient?.ToLowerInvariant());
        }


        public class NerPrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabel { get; set; }
        }


            public static (Tensorflow.Tensor input, Tensorflow.Tensor labels, Tensorflow.Tensor loss, Tensorflow.Tensor trainOp, Tensorflow.Tensor prediction) BuildBiLstmSoftmaxModel(
    int vocabSize, int tagCount, int maxSeqLen, int embeddingDim = 128)
            {
                tf.compat.v1.disable_eager_execution();

                var input = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "input");
                var labels = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "labels");

                // Embedding layer
                var embeddingMatrix = tf.Variable(tf.random.uniform((vocabSize, embeddingDim), -1.0f, 1.0f));
                var embedded = tf.nn.embedding_lookup((IVariableV1)embeddingMatrix, input);

            // BiLSTM layer
            var lstmFw = tf.keras.layers.LSTM(64, return_sequences: true,
        kernel_initializer: tf.random_uniform_initializer);

            var lstmBw = tf.keras.layers.LSTM(64, return_sequences: true, go_backwards: true,
                kernel_initializer: tf.random_uniform_initializer);
            var lstmFwOut = lstmFw.Apply(embedded);
                var lstmBwOut = lstmBw.Apply(embedded);

                var concat = tf.concat(new List<Tensorflow.Tensor> { lstmFwOut, lstmBwOut }, axis: -1); // shape: [batch, seq_len, 128]

                // Dense layer
                var logits = tf.keras.layers.Dense(units: tagCount).Apply(concat); // shape: [batch, seq_len, tagCount]
                var prediction = tf.math.argmax(logits, axis: -1); // shape: [batch, seq_len]

                // Loss
                var flatLogits = tf.reshape(logits, (-1, tagCount));
                var flatLabels = tf.reshape(labels, (-1));
                var loss = tf.reduce_mean(tf.nn.sparse_softmax_cross_entropy_with_logits(labels: flatLabels, logits: flatLogits));

                var trainOp = tf.train.AdamOptimizer(learning_rate: 0.001f).minimize(loss);

                return (input, labels, loss, trainOp, prediction);
            }

            public static void TrainAndPredictBilstmModel(NDArray X, NDArray y, NDArray inputMatrix,
                                                  int vocabSize, int tagCount, int maxSeqLen)
            {
                // Build the model
                var (input, labels, loss, trainOp, prediction) = BuildBiLstmSoftmaxModel(vocabSize, tagCount, maxSeqLen);

                // Start session
                using var sess = tf.Session();
                sess.run(tf.global_variables_initializer());

                // Train
                for (int epoch = 0; epoch < 50; epoch++)
                {
                    var (_, currLoss) = sess.run((trainOp, loss),
                        new FeedItem(input, X.astype(np.int32)),
                        new FeedItem(labels, y.astype(np.int32)));

                    Console.WriteLine($"Epoch {epoch + 1}: Loss = {currLoss:0.####}");
                }

                Console.WriteLine("\n✅ Training complete.\n");

                // Predict
                var testInput = inputMatrix; // shape [1, maxSeqLen]
                var predOutput = sess.run(prediction, new FeedItem(input, testInput));
                var tagIds = ((NDArray)predOutput)[0].ToArray<int>();

                Console.WriteLine("🔍 Predicted tag IDs:");
                Console.WriteLine(string.Join(" ", tagIds));
            }

        

        public static List<NerTokenRow> FlattenBioJson(string jsonFilePath)
        {
            var sentences = JsonConvert.DeserializeObject<List<BioSentence>>(File.ReadAllText(jsonFilePath));
            var rows = new List<NerTokenRow>();

            for (int i = 0; i < sentences.Count; i++)
            {
                var sentence = sentences[i];
                foreach (var token in sentence.Tokens)
                {
                    rows.Add(new NerTokenRow
                    {
                        SentenceId = i,
                        Token = token.Token,
                        Tag = token.Tag
                    });
                }
            }

            return rows;
        }

        public static List<TokenInput> ConvertToTokenInputWithContext(List<NerTokenRow> flatData)
        {
            var grouped = flatData.GroupBy(x => x.SentenceId).ToList();
            var result = new List<TokenInput>();

            foreach (var sentence in grouped)
            {
                var tokens = sentence.ToList();

                for (int i = 0; i < tokens.Count; i++)
                {
                    var prev = i > 0 ? tokens[i - 1].Token : "<START>";
                    var curr = tokens[i].Token;
                    var next = i < tokens.Count - 1 ? tokens[i + 1].Token : "<END>";
                    var label = tokens[i].Tag;

                    result.Add(new TokenInput
                    {
                        PrevToken = prev,
                        Token = curr,
                        NextToken = next,
                        Label = label
                    });
                }
            }

            return result;
        }

        public class BioSentence
        {
            public List<BioToken> Tokens { get; set; }
        }

        public class BioToken
        {
            public string Token { get; set; }
            public string Tag { get; set; }
        }

        public static List<BioTaggedSentence> LoadFlatTSVAsTaggedSentences(string filePath)
        {
            var result = new List<BioTaggedSentence>();
            var currentTokens = new List<BioTaggedToken>(); // Change type to match BioTaggedToken  

            foreach (var line in File.ReadLines(filePath).Skip(1)) // skip header  
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (currentTokens.Count > 0)
                    {
                        result.Add(new BioTaggedSentence { Tokens = new List<BioTaggedToken>(currentTokens) }); // Fix type mismatch  
                        currentTokens.Clear();
                    }
                    continue;
                }

                var parts = line.Split('\t');
                if (parts.Length != 4) continue;

                var token = parts[1]; // Current token  
                var tag = parts[3];   // BIO tag  

                currentTokens.Add(new BioTaggedToken { Token = token, Tag = tag }); // Fix type mismatch  
            }

            // Add any leftover sentence  
            if (currentTokens.Count > 0)
            {
                result.Add(new BioTaggedSentence { Tokens = currentTokens });
            }

            return result;
        }


        public static (NDArray X, NDArray y, Dictionary<string, int> word2idx, Dictionary<string, int> tag2idx)
    EncodeTaggedSentences(List<BioTaggedSentence> sentences, int maxSeqLen)
        {
            var X = np.zeros((sentences.Count, maxSeqLen), dtype: np.int32);
            var y = np.zeros((sentences.Count, maxSeqLen), dtype: np.int32);

            var word2idx = new Dictionary<string, int> { ["<PAD>"] = 0, ["<UNK>"] = 1 };
            var tag2idx = new Dictionary<string, int> { ["O"] = 0 };

            foreach (var sentence in sentences)
            {
                foreach (var token in sentence.Tokens)
                {
                    if (!word2idx.ContainsKey(token.Token.ToLower()))
                        word2idx[token.Token.ToLower()] = word2idx.Count;
                    if (!tag2idx.ContainsKey(token.Tag))
                        tag2idx[token.Tag] = tag2idx.Count;
                }
            }

            for (int i = 0; i < sentences.Count; i++)
            {
                var s = sentences[i];
                for (int j = 0; j < Math.Min(maxSeqLen, s.Tokens.Count); j++)
                {
                    var token = s.Tokens[j].Token.ToLower();
                    var tag = s.Tokens[j].Tag;

                    X[i, j] = word2idx.TryGetValue(token, out var id) ? id : word2idx["<UNK>"];
                    y[i, j] = tag2idx[tag];
                }
            }

            return (X, y, word2idx, tag2idx);
        }

        public static (int TP, int FP, int FN) ComputeNERMetrics(string[] trueTags, string[] predictedTags, string tag)
        {
            int tp = 0, fp = 0, fn = 0;

            for (int i = 0; i < trueTags.Length; i++)
            {
                if (predictedTags[i] == tag && trueTags[i] == tag) tp++;
                else if (predictedTags[i] == tag && trueTags[i] != tag) fp++;
                else if (predictedTags[i] != tag && trueTags[i] == tag) fn++;
            }
            return (tp, fp, fn);
        }
        public static void TrainModel()
        {
            tf.compat.v1.disable_eager_execution();

            int embeddingDim = 128;
            int maxSeqLen = 100;

            // Load and prepare data
            //var dataset = LoadBIOAnnotatedDataset("bio_annotated_dataset_old_new3.json");

            var dataset = LoadFlatTSVAsTaggedSentences("bio_dataset_flat3.tsv");


            Console.WriteLine($"Loaded sentences: {dataset.Count}");
            var (X, y, word2idx, tag2idx) = PrepareData(dataset, maxSeqLen);

            Console.WriteLine($"Original y shape: {y.shape}");
            var reshapedY = y.reshape(-1);
            Console.WriteLine($"Flattened y shape: {reshapedY.shape}");
            int vocabSize = word2idx.Count;
            int tagCount = tag2idx.Count;

            // Placeholders
            var input = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "input");
            var labels = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "labels");

            // Embedding Layer
            var embeddingMatrix = tf.Variable(tf.random.uniform((vocabSize, embeddingDim), -1.0f, 1.0f), name: "embedding_matrix");
            var embedded = tf.nn.embedding_lookup((Tensorflow.Tensor)embeddingMatrix, input);

            // Flatten and Dense
            //var flatten = tf.reshape(embedded, (-1, maxSeqLen * embeddingDim));
            //var weights = tf.Variable(tf.random.truncated_normal((maxSeqLen * embeddingDim, tagCount), stddev: 0.1f));

            var W = tf.Variable(tf.random.truncated_normal((embeddingDim, tagCount), stddev: 0.1f));
            var b = tf.Variable(tf.zeros(tagCount));

            // embedded: [batch, seq_len, embeddingDim]
            // reshape to 2D for matmul
            var embedded2D = tf.reshape(embedded, (-1, embeddingDim));          // [batch * seq_len, embDim]
            var logits2D = tf.matmul(embedded2D, W) + b;                         // [batch * seq_len, tagCount]
            var logits3D = tf.reshape(logits2D, (-1, maxSeqLen, tagCount));
            //var biases = tf.Variable(tf.zeros(tagCount));
            //var logits = tf.matmul(flatten, weights) + biases;
           // logits = tf.reshape(logits, (-1, maxSeqLen, tagCount)); // [batch, seq_len, tagCount]

            // Reshape to [batch * seq_len, tagCount] and [batch * seq_len]
            var flatLogits = tf.reshape(logits3D, (-1, tagCount));
            var flatLabels = tf.reshape(labels, (-1));

            // Loss & optimizer
            var loss = tf.reduce_mean(tf.nn.sparse_softmax_cross_entropy_with_logits(labels: flatLabels, logits: flatLogits));
            var train_op = tf.train.AdamOptimizer(0.001f).minimize(loss);

            // Session
            using var sess = tf.Session();
            sess.run(tf.global_variables_initializer());

            // Use X and y as-is
            var feedX = X.astype(np.int32);
            var feedY = y.astype(np.int32);
            var flatY = y.reshape(-1);
            var sw = Stopwatch.StartNew();
            int patience = 5; // how many times to allow stagnation
            double minDelta = 1e-4; // minimum change considered an improvement
            double bestLoss = double.MaxValue;
            int wait = 0;

            for (int epoch = 0; epoch < 150; epoch++)
            {
                var (_, curr_loss) = sess.run((train_op, loss),
                    new FeedItem(input, feedX),
                    new FeedItem(labels, flatY));

                Console.WriteLine($"Epoch {epoch + 1}: Loss = {curr_loss:0.####}");

              
            }
            sw.Stop();
            Console.WriteLine($"⏱ Training time: {sw.Elapsed.TotalSeconds:0.###} seconds");

            Console.WriteLine("✅ Model training complete.");

            // Save model
            var saver = tf.train.Saver();
            saver.save(sess, "./ner_model.ckpt");

            var idx2tag = tag2idx.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);



            var testExamples = new List<NerTestSample>
{
    new() {
        Tokens = new[] { "replace", "cream", "cheese", "with", "quark" },
        Labels = new[] { "O", "B-OLD", "I-OLD", "O", "B-NEW" }
    },
    new() {
        Tokens = new[] { "can", "I", "use", "tempeh", "instead", "of", "bacon" },
        Labels = new[] { "O", "O", "O", "B-NEW", "O", "O", "B-OLD" }
    },
    new() {
        Tokens = new[] { "try", "ghee", "instead", "of", "olive", "oil" },
        Labels = new[] { "O", "B-NEW", "O", "O", "B-OLD", "I-OLD" }
    },
    new() {
        Tokens = new[] { "get", "rid", "of", "white", "sugar" },
        Labels = new[] { "O", "O", "O", "B-OLD", "I-OLD" }
    },
    new() {
        Tokens = new[] { "add", "chia", "seeds" },
        Labels = new[] { "O", "B-NEW", "I-NEW" }
    },
    new() {
        Tokens = new[] { "use", "coconut", "nectar", "as", "a", "sweetener" },
        Labels = new[] { "O", "B-NEW", "I-NEW", "O", "O", "O" }
    },
    new() {
        Tokens = new[] { "remove", "whey", "protein" },
        Labels = new[] { "O", "B-OLD", "I-OLD" }
    },
    new() {
        Tokens = new[] { "swap", "paneer", "for", "tofu" },
        Labels = new[] { "O", "B-OLD", "O", "B-NEW" }
    },
    new() {
        Tokens = new[] { "I", "want", "to", "avoid", "mayonnaise" },
        Labels = new[] { "O", "O", "O", "O", "B-OLD" }
    },
    new() {
        Tokens = new[] { "can", "you", "replace", "cashew", "milk", "with", "soy", "milk" },
        Labels = new[] { "O", "O", "O", "B-OLD", "I-OLD", "O", "B-NEW", "I-NEW" }
    }
};

            Console.WriteLine("\n📊 Evaluating TensorFlow.NET on test examples...");

            Dictionary<string, (int TP, int FP, int FN)> tfMetrics = new();

            foreach (var sample in testExamples)
            {
                var predicted = PredictTagsTF(sample.Tokens, word2idx, sess, input, logits3D, idx2tag);

                for (int i = 0; i < sample.Tokens.Length; i++)
                {
                    var trueLabel = sample.Labels[i];
                    var pred = predicted[i];

                    if (!tfMetrics.ContainsKey(trueLabel)) tfMetrics[trueLabel] = (0, 0, 0);
                    if (!tfMetrics.ContainsKey(pred)) tfMetrics[pred] = (0, 0, 0);

                    if (trueLabel == pred)
                        tfMetrics[trueLabel] = (tfMetrics[trueLabel].TP + 1, tfMetrics[trueLabel].FP, tfMetrics[trueLabel].FN);
                    else
                    {
                        tfMetrics[pred] = (tfMetrics[pred].TP, tfMetrics[pred].FP + 1, tfMetrics[pred].FN);
                        tfMetrics[trueLabel] = (tfMetrics[trueLabel].TP, tfMetrics[trueLabel].FP, tfMetrics[trueLabel].FN + 1);
                    }
                }
            }

            foreach (var tag in tfMetrics.Keys)
            {
                var (tp, fp, fn) = tfMetrics[tag];
                double precision = tp / (double)(tp + fp + 1e-6);
                double recall = tp / (double)(tp + fn + 1e-6);
                double f1 = 2 * precision * recall / (precision + recall + 1e-6);
                Console.WriteLine($"{tag,-6} => P: {precision:0.###}, R: {recall:0.###}, F1: {f1:0.###}");
            }

            var sentences = LoadFlatTSVAsTaggedSentences("bio_dataset_flat3.tsv");
             maxSeqLen = 100;

             (X, y, word2idx, tag2idx) = EncodeTaggedSentences(sentences, maxSeqLen);
            var inputMatrix = X[$"{0}:" + "1"]; // shape [1, maxSeqLen] — first sentence

             vocabSize = word2idx.Count;
             tagCount = tag2idx.Count;

            TrainAndPredictBilstmModel(X, y, inputMatrix, vocabSize, tagCount, maxSeqLen);

            //var idx2tag = tag2idx.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            //// 🔮 Predict on a sample sentence
            //string testSentence = "replace sugar with stevia";
            //var prediction = PredictNER(testSentence, word2idx, idx2tag, sess, input, logits3D);

            //Console.WriteLine($"\n🔎 Prediction for: \"{testSentence}\"");
            //Console.WriteLine($"Old Ingredient: {prediction.OldIngredient}");
            //Console.WriteLine($"New Ingredient: {prediction.NewIngredient}");
        }

        public static string[] PredictTagsTF(string[] tokens, Dictionary<string, int> word2idx, Session session, Tensorflow.Tensor input, Tensorflow.Tensor logits, Dictionary<int, string> idx2tag)
        {
            var inputIds = tokens.Select(t => word2idx.TryGetValue(t.ToLower(), out var id) ? id : word2idx["<UNK>"]).ToList();
            while (inputIds.Count < 100) inputIds.Add(word2idx["<PAD>"]);
            inputIds = inputIds.Take(100).ToList();

            var input2D = new int[1, 100];
            for (int i = 0; i < 100; i++) input2D[0, i] = inputIds[i];
            var inputArr = np.array(input2D);

            var output = session.run(logits, new FeedItem(input, inputArr));
            var predIds = ((NDArray)np.argmax(output, axis: -1)).astype(np.int32)[0].ToArray<int>();
            return tokens.Select((t, i) => i < predIds.Length && idx2tag.ContainsKey(predIds[i]) ? idx2tag[predIds[i]] : "O").ToArray();
        }

        public static void TrainBiLSTMModel()
        {
            int embeddingDim = 128;
            int maxSeqLen = 100;
            int batchSize = 32;
            int numEpochs = 300;
            float learningRate = 0.001f;

            // Load and prepare data
            var dataset = LoadFlatTSVAsTaggedSentences("bio_dataset_flat3.tsv");
            Console.WriteLine($"Loaded sentences: {dataset.Count}");

            var (X, yRaw, word2idx, tag2idx) = PrepareData(dataset, maxSeqLen);
            int vocabSize = word2idx.Count;
            int tagCount = tag2idx.Count;

            // One-hot encode y [samples, seqLen] → [samples, seqLen, tagCount]
            var yOneHotTensor = tf.one_hot(yRaw, depth: tagCount);
            var yOneHot = yOneHotTensor.numpy(); // cast to NDArray for fit()

            // Define model
            //var keras = tf.keras;
            //var inputs = keras.Input(shape: new Shape(maxSeqLen), dtype: tf.@int32);
            //var x = keras.layers.Embedding(input_dim: vocabSize, output_dim: embeddingDim, mask_zero: true).Apply(inputs);


            //x = keras.layers.Bidirectional(keras.layers.LSTM(64, return_sequences: true)).Apply(x);
            //x = keras.layers.Dropout(rate: 0.3f).Apply(x); // Add dropout for regularization
            //x = keras.layers.Dense(tagCount, activation: "softmax").Apply(x);


            var keras = tf.keras;
            var inputs = keras.Input(shape: new Shape(maxSeqLen), dtype: tf.int32);
            var x = keras.layers.Embedding(vocabSize, embeddingDim).Apply(inputs);
            x = keras.layers.Dense(64, activation: "relu").Apply(x);
            x = keras.layers.Dropout(0.3f).Apply(x);
            x = keras.layers.Dense(tagCount, activation: "softmax").Apply(x);

            var model = keras.Model(inputs, x);

            model.compile(optimizer: keras.optimizers.Adam(learningRate),
                          loss: keras.losses.CategoricalCrossentropy(from_logits: false),
                          metrics: new[] { "accuracy" });

            // Cast input
            X = X.astype(np.int32);

            // Train
            try
            {
                model.fit(X, yOneHot, batch_size: batchSize, epochs: numEpochs, validation_split: 0.1f);
                Console.WriteLine("✅ Training finished.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Training crashed: " + ex.Message);
                return;
            }

            //// Prediction
            var idx2tag = tag2idx.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            //string testSentence = "trade quinoa for rice";

            //var testX = PrepareTestInput2(testSentence, word2idx, maxSeqLen);
            //testX = testX.astype(np.int32);



            //try
            //{
            //    var prediction = model.Apply(testX);
            //    var predictionNp = prediction.numpy();
            //    var predictedIndices = np.argmax(predictionNp, axis: -1).astype(np.int32).ToArray<int>();

            //    var tokens = Tokenize(testSentence);
            //    Console.WriteLine("Predicted tags:");
            //    for (int i = 0; i < tokens.Count && i < predictedIndices.Length; i++)
            //    {
            //        Console.WriteLine($"{tokens[i]}: {idx2tag[predictedIndices[i]]}");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("❌ Prediction failed: " + ex.Message);
            //}


            // Evaluation on custom test samples
            Console.WriteLine("\n📊 Evaluating TensorFlow.NET on test examples...");

            var testExamples = new List<(string Sentence, string[] TrueLabels)>
    {
        ("replace sugar with stevia", new[] { "O", "B-OLD", "O", "B-NEW" }),
        ("substitute butter with margarine", new[] { "O", "B-OLD", "O", "B-NEW" }),
        ("swap milk for almond milk", new[] { "O", "B-OLD", "O", "B-NEW", "I-NEW" }),
        ("I want to get rid of milk", new[] { "O", "O", "O", "O", "O", "O", "B-OLD" }),
        ("include honey", new[] { "O", "B-NEW" }),
       (
    "replace cream cheese with quark",
    new[] { "O", "B-OLD", "I-OLD", "O", "B-NEW" }
),
(
    "can I use tempeh instead of bacon",
    new[] { "O", "O", "O", "B-NEW", "O", "O", "B-OLD" }
),
(
    "try ghee instead of olive oil",
    new[] { "O", "B-NEW", "O", "O", "B-OLD", "I-OLD" }
),
(
    "get rid of white sugar",
    new[] { "O", "O", "O", "B-OLD", "I-OLD" }
),
(
    "add chia seeds",
    new[] { "O", "B-NEW", "I-NEW" }
),
(
    "use coconut nectar as a sweetener",
    new[] { "O", "B-NEW", "I-NEW", "O", "O", "O" }
),
(
    "remove whey protein",
    new[] { "O", "B-OLD", "I-OLD" }
),
(
    "swap paneer for tofu",
    new[] { "O", "B-OLD", "O", "B-NEW" }
),
(
    "I want to avoid mayonnaise",
    new[] { "O", "O", "O", "O", "B-OLD" }
),
(
    "can you replace cashew milk with soy milk",
    new[] { "O", "O", "O", "B-OLD", "I-OLD", "O", "B-NEW", "I-NEW" }
)

    };



            var metrics = new Dictionary<string, (int TP, int FP, int FN)>();

            foreach (var (sentence, trueLabels) in testExamples)
            {
                var testX = PrepareTestInput2(sentence, word2idx, maxSeqLen).astype(np.int32);
                var prediction = model.Apply(testX).numpy();
                var predictedIndices = np.argmax(prediction, axis: -1).astype(np.int32).ToArray<int>();

                var tokens = Tokenize(sentence);
                for (int i = 0; i < tokens.Count && i < trueLabels.Length; i++)
                {
                    string predictedTag = idx2tag[predictedIndices[i]];
                    string trueTag = trueLabels[i];

                    if (!metrics.ContainsKey(trueTag)) metrics[trueTag] = (0, 0, 0);
                    if (!metrics.ContainsKey(predictedTag)) metrics[predictedTag] = (0, 0, 0);

                    if (predictedTag == trueTag)
                        metrics[trueTag] = (metrics[trueTag].TP + 1, metrics[trueTag].FP, metrics[trueTag].FN);
                    else
                    {
                        metrics[predictedTag] = (metrics[predictedTag].TP, metrics[predictedTag].FP + 1, metrics[predictedTag].FN);
                        metrics[trueTag] = (metrics[trueTag].TP, metrics[trueTag].FP, metrics[trueTag].FN + 1);
                    }
                }
            }

            foreach (var tag in metrics.Keys)
            {
                var (tp, fp, fn) = metrics[tag];
                double precision = tp / (double)(tp + fp + 1e-6);
                double recall = tp / (double)(tp + fn + 1e-6);
                double f1 = 2 * precision * recall / (precision + recall + 1e-6);
                Console.WriteLine($"{tag,-6} => P: {precision:0.###}, R: {recall:0.###}, F1: {f1:0.###}");
            }
        }



        public static NDArray PrepareTestInput(string sentence, Dictionary<string, int> word2idx, int maxSeqLen)
        {
            // Tokenize the input using whitespace or other logic (replace with custom tokenizer if needed)
            var tokens = sentence.ToLowerInvariant().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Convert tokens to indices
            var indices = tokens.Select(token => word2idx.TryGetValue(token, out var id) ? id : word2idx["<UNK>"]).ToList();

            // Pad or truncate
            if (indices.Count < maxSeqLen)
            {
                indices.AddRange(Enumerable.Repeat(word2idx["<PAD>"], maxSeqLen - indices.Count));
            }
            else if (indices.Count > maxSeqLen)
            {
                indices = indices.Take(maxSeqLen).ToList();
            }

            // Reshape to [1, maxSeqLen] for batch inference
            var inputArray = np.array(indices.ToArray()).reshape(new Shape(1, maxSeqLen));

            return inputArray;
        }

        public static NDArray PrepareTestInput2(string sentence, Dictionary<string, int> word2idx, int maxSeqLen)
        {
            var tokens = Regex.Matches(sentence, @"\w+|[^\w\s]")
                       .Select(m => m.Value.ToLowerInvariant())
                       .ToList();

            var ids = tokens.Select(token => word2idx.TryGetValue(token, out var id) ? id : word2idx["<UNK>"]).ToList();

            // Pad or truncate to maxSeqLen
            while (ids.Count < maxSeqLen)
                ids.Add(word2idx["<PAD>"]);
            ids = ids.Take(maxSeqLen).ToList();

            // Build a 2D array for batch size 1
            var input2D = new int[1, maxSeqLen];
            for (int i = 0; i < maxSeqLen; i++)
                input2D[0, i] = ids[i];

            var ndarray = np.array(input2D);
            return ndarray.astype(np.int32);
        }

        public static List<string> Tokenize(string sentence)
        {
            return sentence.Split(' ').ToList();
        }

        public static void SaveVocabulary(Dictionary<string, int> word2idx, Dictionary<string, int> tag2idx, string path)
        {
            var vocabData = new VocabData
            {
                WordToIdx = word2idx,
                TagToIdx = tag2idx
            };

            var json = System.Text.Json.JsonSerializer.Serialize(vocabData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(path, json);
        }

        // Using the predictor
        private static NERPredictor predictor;
        public static void TestPrediction()
        {
            try
            {
                predictor = new NERPredictor(
                    modelPath: "./ner_model",
                    vocabPath: "./vocab.json"
                );

                while (true)
                {
                    Console.Write("\nEnter a sentence (or press Enter to exit): ");
                    var input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input)) break;

                    var result = predictor.Predict(input);
                    Console.WriteLine($"\nOld Ingredient: {result.OldIngredient}");
                    Console.WriteLine($"New Ingredient: {result.NewIngredient}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static IngredientEntities PredictNER(
            string sentence,
            Dictionary<string, int> word2idx,
            Dictionary<int, string> idx2tag,
            Session session,
            Tensorflow.Tensor inputTensor,
            Tensorflow.Tensor logitsTensor,
            int maxSeqLen = 100)
        {
            // Use the same regex-based tokenization as in training
            var tokens = Regex.Matches(sentence, @"\w+|[^\w\s]")
                              .Select(m => m.Value.ToLowerInvariant())
                              .ToArray();
            var inputIds = tokens
                .Select(token => word2idx.TryGetValue(token, out var id) ? id : word2idx["<UNK>"])
                .ToList();

            // Pad or truncate
            while (inputIds.Count < maxSeqLen)
                inputIds.Add(word2idx["<PAD>"]);
            inputIds = inputIds.Take(maxSeqLen).ToList();

            // Prepare input tensor
            var input2D = new int[1, maxSeqLen];
            for (int i = 0; i < maxSeqLen; i++)
                input2D[0, i] = inputIds[i];
            var inputArr = np.array(input2D);

            // Predict
            var logits = session.run(logitsTensor, new FeedItem(inputTensor, inputArr));
            var predictions = ((NDArray)np.argmax(logits, axis: -1))
                .astype(np.int32)[0]
                .ToArray<int>();

            // Map predictions to tags
            var tags = tokens.Select((t, i) => i < predictions.Length && idx2tag.ContainsKey(predictions[i]) ? idx2tag[predictions[i]] : "O").ToArray();

            Console.WriteLine("\n🧠 Predicted BIO Tags:");
            for (int i = 0; i < tokens.Length; i++)
            {
                Console.WriteLine($"  {tokens[i]} => {tags[i]}");
            }

            //// Post-process for substitute patterns
            //var substituteRegex = new Regex(@"replace\s+([\w\s-]+?)\s+with\s+([\w\s-]+)", RegexOptions.IgnoreCase);
            //var match = substituteRegex.Match(sentence);
            //if (match.Success)
            //{
            //    var oldIng = match.Groups[1].Value.Trim();
            //    var newIng = match.Groups[2].Value.Trim();
            //    return new IngredientEntities
            //    {
            //        OldIngredient = oldIng,
            //        NewIngredient = newIng
            //    };
            //}

            // Fallback to tag-based extraction for other cases

            var trueTags = new[] { "O", "B-OLD", "O", "B-NEW" }; // Manually labeled or loaded
            var predictedTags = tags; // From PredictNER

            var (tp, fp, fn) = ComputeNERMetrics(trueTags, predictedTags, "B-OLD");
            double precision = tp / (double)(tp + fp + 1e-5);
            double recall = tp / (double)(tp + fn + 1e-5);
            double f1 = 2 * precision * recall / (precision + recall + 1e-5);

            Console.WriteLine($"B-OLD → Precision: {precision:0.###}, Recall: {recall:0.###}, F1: {f1:0.###}");
            return ProcessPredictions(tokens, tags);
        }

        public class NerTestSample
        {
            public string[] Tokens { get; set; }
            public string[] Labels { get; set; }
        }



        private static IngredientEntities ProcessPredictions(string[] tokens, string[] tags)
        {
            string oldIngredient = "";
            string newIngredient = "";
            string currentOld = "";
            string currentNew = "";
            bool inOld = false;
            bool inNew = false;

            for (int i = 0; i < tokens.Length; i++)
            {
                var tag = tags[i];
                var token = tokens[i];
                if (tag == "B-OLD")
                {
                    if (!string.IsNullOrWhiteSpace(currentOld))
                    {
                        oldIngredient = currentOld.Trim();
                    }
                    currentOld = token;
                    inOld = true;
                    inNew = false;
                }
                else if (tag == "I-OLD" && inOld)
                {
                    currentOld += " " + token;
                }
                else if (tag == "B-NEW")
                {
                    if (!string.IsNullOrWhiteSpace(currentNew))
                    {
                        newIngredient = currentNew.Trim();
                    }
                    currentNew = token;
                    inNew = true;
                    inOld = false;
                }
                else if (tag == "I-NEW" && inNew)
                {
                    currentNew += " " + token;
                }
                else
                {
                    inOld = false;
                    inNew = false;
                }
            }
            // Finalize last entity
            if (!string.IsNullOrWhiteSpace(currentOld))
                oldIngredient = currentOld.Trim();
            if (!string.IsNullOrWhiteSpace(currentNew))
                newIngredient = currentNew.Trim();
            return new IngredientEntities
            {
                OldIngredient = string.IsNullOrWhiteSpace(oldIngredient) ? null : oldIngredient,
                NewIngredient = string.IsNullOrWhiteSpace(newIngredient) ? null : newIngredient
            };
        }
        public static (NDArray X, NDArray y, Dictionary<string, int> word2idx, Dictionary<string, int> tag2idx) PrepareData(
    List<BioTaggedSentence> data,
    int maxSeqLen = 100)
        {
            var word2idx = new Dictionary<string, int> { ["<PAD>"] = 0, ["<UNK>"] = 1 };
            var tag2idx = new Dictionary<string, int> { ["O"] = 0 };

            var X = new List<List<int>>();
            var Y = new List<List<int>>();

            foreach (var sentence in data)
            {
                var wordIds = new List<int>();
                var tagIds = new List<int>();

                foreach (var token in sentence.Tokens)
                {
                    var word = token.Token.ToLowerInvariant();
                    var tag = token.Tag;

                    if (!word2idx.ContainsKey(word))
                        word2idx[word] = word2idx.Count;
                    if (!tag2idx.ContainsKey(tag))
                        tag2idx[tag] = tag2idx.Count;

                    wordIds.Add(word2idx[word]);
                    tagIds.Add(tag2idx[tag]);
                }

                // Pad or truncate
                if (wordIds.Count < maxSeqLen)
                {
                    wordIds.AddRange(Enumerable.Repeat(word2idx["<PAD>"], maxSeqLen - wordIds.Count));
                    tagIds.AddRange(Enumerable.Repeat(tag2idx["O"], maxSeqLen - tagIds.Count));
                }
                else
                {
                    wordIds = wordIds.Take(maxSeqLen).ToList();
                    tagIds = tagIds.Take(maxSeqLen).ToList();
                }

                X.Add(wordIds);
                Y.Add(tagIds);
            }

            // Convert to NDArray
            var XArr = np.array(To2DArray(X)).astype(np.int32);
            var YArr = np.array(To2DArray(Y)).astype(np.int32); // ✅ NO expand_dims

            return (XArr, YArr, word2idx, tag2idx);

        
        }
        public static int[,] To2DArray(List<List<int>> list)
        {
            int rows = list.Count;
            int cols = list[0].Count;
            int[,] array = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    array[i, j] = list[i][j];
            return array;
        }

        static List<BioTaggedSentence> LoadBIOAnnotatedDataset(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<BioTaggedSentence>>(json);
        }

        public static void TrainModelForTextClassification()
        {
            var context = new MLContext();

            string filePath = "nlp_recipe_intents_15000.txt";
            var data = context.Data.LoadFromTextFile<ModelInput>(filePath, separatorChar: '\t', hasHeader: true);

            var split = context.Data.TrainTestSplit(data, testFraction: 0.2);
            var pipeline = context.Transforms.Conversion.MapValueToKey("Label")
                 .Append(context.Transforms.Text.FeaturizeText("Features", "Text"))
                 .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                 .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(split.TrainSet);


            var predictions = model.Transform(split.TestSet);
            var metrics = context.MulticlassClassification.Evaluate(predictions);
            Console.WriteLine($"Accuracy: {metrics.MicroAccuracy:P2}");


            context.Model.Save(model, split.TrainSet.Schema, "IntentClassifier.zip");
        }

        public static void PredictUserIntent()
        {
            var context = new MLContext();

            // TrainModelForTextClassification();

            // Create prediction engine
            var predictor = context.Model.Load("IntentClassifier.zip", out var schema);
            var engine = context.Model.CreatePredictionEngine<ModelInput, ModelOutput>(predictor);

            // Test it
            while (true)
            {
                Console.Write("\nType a user command: ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                var result = engine.Predict(new ModelInput { Text = input });
                Console.WriteLine($"Predicted intent: {result.PredictedIntent}");
            }
        }
    }

    public class NERPredictor
    {
        private Session session;
        private Tensorflow.Tensor inputTensor;
        private Tensorflow.Tensor logitsTensor;
        private Dictionary<string, int> word2idx;
        private Dictionary<int, string> idx2tag;
        private const int maxSeqLen = 100;

        public NERPredictor(string modelPath, string vocabPath)
        {
            tf.compat.v1.disable_eager_execution();
            LoadVocabulary(vocabPath);
            InitializeModel();
            RestoreModel(modelPath);
        }

        private void LoadVocabulary(string vocabPath)
        {
            // Load your vocabulary from saved file
            var vocabData = System.Text.Json.JsonSerializer.Deserialize<VocabData>(File.ReadAllText(vocabPath));
            word2idx = vocabData.WordToIdx;
            idx2tag = vocabData.TagToIdx.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        private void InitializeModel()
        {
            try
            {
                tf.reset_default_graph();
                int embeddingDim = 128;
                int vocabSize = word2idx.Count;
                int tagCount = idx2tag.Count;

                // Input placeholder
                inputTensor = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "input");

                // Embedding layer
                var embeddingMatrix = tf.Variable(
                    tf.random.uniform((vocabSize, embeddingDim), -1.0f, 1.0f),
                    name: "embedding_matrix"
                );
                var embedded = tf.nn.embedding_lookup((Tensorflow.Tensor)embeddingMatrix, inputTensor);

                // Dense layer
                var flattenedEmbedded = tf.reshape(embedded, new Shape(-1, embeddingDim));
                var weights = tf.Variable(
                    tf.random.truncated_normal((embeddingDim, tagCount), stddev: 0.1f),
                    name: "dense_weights"
                );
                var biases = tf.Variable(
                    tf.zeros(tagCount),
                    name: "dense_biases"
                );

                var logits2D = tf.matmul(flattenedEmbedded, weights) + biases;
                logitsTensor = tf.reshape(logits2D, new Shape(-1, maxSeqLen, tagCount));

                Console.WriteLine("Model initialized successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing model: {ex.Message}");
                throw;
            }
        }

        private void RestoreModel(string modelPath)
        {
            try
            {
                session = tf.Session();
                session.run(tf.global_variables_initializer());

                // Create saver without explicitly specifying variables
                var saver = tf.train.Saver();
                saver.restore(session, modelPath);

                Console.WriteLine("Model restored successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring model: {ex.Message}\nStack trace: {ex.StackTrace}");
                throw;
            }
        }

        public IngredientEntities Predict(string sentence)
        {
            try
            {
                // 1. Tokenize using regex (same as training)
                var tokens = Regex.Matches(sentence, @"\w+|[^\w\s]")
                    .Select(m => m.Value.ToLowerInvariant())
                    .ToArray();
                var inputIds = tokens
                    .Select(token => word2idx.TryGetValue(token, out var id) ? id : word2idx["<UNK>"])
                    .ToList();

                // 2. Pad or truncate
                while (inputIds.Count < maxSeqLen)
                    inputIds.Add(word2idx["<PAD>"]);
                inputIds = inputIds.Take(maxSeqLen).ToList();

                // 3. Prepare input tensor
                var input2D = new int[1, maxSeqLen];
                for (int i = 0; i < maxSeqLen; i++)
                    input2D[0, i] = inputIds[i];
                var inputArr = np.array(input2D);

                // 4. Model inference
                var logits = session.run(logitsTensor, new FeedItem(inputTensor, inputArr));
                var predictions = ((NDArray)np.argmax(logits, axis: -1))
                    .astype(np.int32)[0]
                    .ToArray<int>();

                // 5. Map predictions to tags
                var tags = tokens.Select((t, i) => i < predictions.Length && idx2tag.ContainsKey(predictions[i]) ? idx2tag[predictions[i]] : "O").ToArray();

                // Safety: tags and tokens must have same length
                if (tags.Length > tokens.Length)
                    tags = tags.Take(tokens.Length).ToArray();
                else if (tags.Length < tokens.Length)
                    tags = tags.Concat(Enumerable.Repeat("O", tokens.Length - tags.Length)).ToArray();

                // 6. Post-process for all substitute patterns
                var substitutePatterns = new List<(string pattern, string[] groups)>
                {
                    ("replace\\s+([\\w\\s-]+?)\\s+with\\s+([\\w\\s-]+)", new[] { "old", "new" }),
                    ("use\\s+([\\w\\s-]+?)\\s+instead\\s+of\\s+([\\w\\s-]+)", new[] { "new", "old" }),
                    ("swap\\s+([\\w\\s-]+?)\\s+for\\s+([\\w\\s-]+)", new[] { "old", "new" }),
                    ("prefer\\s+([\\w\\s-]+?)\\s+over\\s+([\\w\\s-]+)", new[] { "new", "old" }),
                    ("change\\s+([\\w\\s-]+?)\\s+to\\s+([\\w\\s-]+)", new[] { "old", "new" }),
                    ("exchange\\s+([\\w\\s-]+?)\\s+with\\s+([\\w\\s-]+)", new[] { "old", "new" }),
                    ("trade\\s+([\\w\\s-]+?)\\s+for\\s+([\\w\\s-]+)", new[] { "old", "new" }),
                };
                foreach (var (pattern, groups) in substitutePatterns)
                {
                    var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                    var match = regex.Match(sentence);
                    if (match.Success)
                    {
                        string oldIng = null, newIng = null;
                        for (int i = 0; i < groups.Length; i++)
                        {
                            var groupIndex = i + 1;
                            if (match.Groups.Count > groupIndex && match.Groups[groupIndex].Success)
                            {
                                if (groups[i] == "old") oldIng = match.Groups[groupIndex].Value.Trim();
                                if (groups[i] == "new") newIng = match.Groups[groupIndex].Value.Trim();
                            }
                        }
                        return new IngredientEntities
                        {
                            OldIngredient = oldIng,
                            NewIngredient = newIng
                        };
                    }
                }

                // 7. Fallback to tag-based extraction for all other cases
                return ProcessPredictions(tokens, tags);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during prediction: {ex.Message}");
                throw;
            }
        }
        private static readonly HashSet<string> RemoveCommands = new(StringComparer.OrdinalIgnoreCase)
{
    "remove", "delete", "take", "out", "eliminate", "get", "rid", "of", "no", "skip", "without"
};

        private static readonly HashSet<string> AddCommands = new(StringComparer.OrdinalIgnoreCase)
{
    "add", "include", "put", "insert", "with", "throw", "in"
};

        private static readonly HashSet<string> SubstituteCommands = new(StringComparer.OrdinalIgnoreCase)
{
    "substitute", "replace", "swap", "switch", "trade", "exchange", "use", "instead"
};

        private static readonly HashSet<string> ConnectorWords = new(StringComparer.OrdinalIgnoreCase)
{
    "with", "for", "to", "by", "instead", "of"
};

        private IngredientEntities ProcessPredictions(string[] tokens, string[] tags)
        {
            string oldIngredient = "";
            string newIngredient = "";
            string currentOld = "";
            string currentNew = "";
            bool inOld = false;
            bool inNew = false;

            for (int i = 0; i < tokens.Length; i++)
            {
                var tag = tags[i];
                var token = tokens[i];
                if (tag == "B-OLD")
                {
                    if (!string.IsNullOrWhiteSpace(currentOld))
                    {
                        oldIngredient = currentOld.Trim();
                    }
                    currentOld = token;
                    inOld = true;
                    inNew = false;
                }
                else if (tag == "I-OLD" && inOld)
                {
                    currentOld += " " + token;
                }
                else if (tag == "B-NEW")
                {
                    if (!string.IsNullOrWhiteSpace(currentNew))
                    {
                        newIngredient = currentNew.Trim();
                    }
                    currentNew = token;
                    inNew = true;
                    inOld = false;
                }
                else if (tag == "I-NEW" && inNew)
                {
                    currentNew += " " + token;
                }
                else
                {
                    inOld = false;
                    inNew = false;
                }
            }
            // Finalize last entity
            if (!string.IsNullOrWhiteSpace(currentOld))
                oldIngredient = currentOld.Trim();
            if (!string.IsNullOrWhiteSpace(currentNew))
                newIngredient = currentNew.Trim();
            return new IngredientEntities
            {
                OldIngredient = string.IsNullOrWhiteSpace(oldIngredient) ? null : oldIngredient,
                NewIngredient = string.IsNullOrWhiteSpace(newIngredient) ? null : newIngredient
            };
        }



        public void Dispose()
        {
            session?.Dispose();
        }
    }

    // Helper class to store vocabulary data
    public class VocabData
    {
        public Dictionary<string, int> WordToIdx { get; set; }
        public Dictionary<string, int> TagToIdx { get; set; }
    }
}
public class IngredientNutrition
{
    [Name("Category")]
    public string NameRaw { get; set; }

    [Name("Data.Kilocalories")]
    public decimal Calories { get; set; }
    [Name("Data.Protein")]
    public decimal Protein { get; set; }
    [Name("Data.Carbohydrate")]
    public decimal Carbs { get; set; }
    [Name("Data.Fat.Total Lipid")]
    public decimal Fat { get; set; }
    public string Name => NameRaw?.Replace("-", " ")?.Trim().ToLowerInvariant();

}

public class RecipeRec
{
    public string Name { get; set; }
    public List<IngredientEntry> Ingredients { get; set; }
}

public class IngredientEntryJson
{
    public string Ingredient { get; set; }
    public string Quantity { get; set; }  // Default quantity in grams
    public string Unit { get; set; }       // Unit like "g", "clove", "shell", etc.
    public decimal Calories { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
    public decimal Protein { get; set; }
}

// Recipe model

public class RecipeJson
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("ingredients")]
    public List<IngredientEntryJson> Ingredients { get; set; }

    [JsonProperty("total_nutrition")]
    public RecipeNutritionalInfo TotalNutrition { get; set; }

    [JsonIgnore] 
    public decimal TotalCalories => Ingredients.Sum(ing => NormalizeQuantity(ing.Quantity) / 100 * ing.Calories);


    private decimal NormalizeQuantity(string quantity)
    {
        decimal grams = 0;

        if (string.IsNullOrEmpty(quantity)) return grams;

        // Example: "2 shells (60g)" => 60g for 2 shells
        var match = Regex.Match(quantity, @"(\d+)(\s*)\((\d+)g\)");

        if (match.Success)
        {
            var count = int.Parse(match.Groups[1].Value);
            grams = int.Parse(match.Groups[3].Value);
            grams /= count;  // Dividing the total grams by the number of shells or cloves
        }
        else
        {
            // If no special case, just try to parse as a number (e.g., "100g" -> 100)
            decimal.TryParse(new string(quantity.Where(char.IsDigit).ToArray()), out grams);  // Extract numeric part
        }

        return grams;
    }
}

public class RecipeNutritionalInfo
{
    [JsonProperty("calories")]
    public float Calories { get; set; }

    [JsonProperty("protein")]
    public float Proteins { get; set; }

    [JsonProperty("carbs")]
    public float Carbs { get; set; }

    [JsonProperty("fat")]
    public float Fats { get; set; }
}

public class IngredientEntry
{
    public string Name { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; }
}


public class ModelInput
{
    [LoadColumn(1)]
    public string Text { get; set; }

    [LoadColumn(0)]
    public string Label { get; set; }
}

public class ModelOutput
{
    [ColumnName("PredictedLabel")]
    public string PredictedIntent { get; set; }
}

public class IngredientEntities
{
    public string? OldIngredient { get; set; }
    public string? NewIngredient { get; set; }
}

public class RecipePredictor
{
    private readonly MLContext _mlContext;
    private readonly ITransformer _model;
    private readonly PredictionEngine<RecipeInput, RecipePrediction> _predictionEngine;

    public RecipePredictor(string modelPath)
    {
        _mlContext = new MLContext();
        _model = _mlContext.Model.Load(modelPath, out var _);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<RecipeInput, RecipePrediction>(_model);
    }

    public RecipeRecommendationResult Predict(string userInput, List<RecipeJson> availableRecipes)
    {
        try
        {
            // Prepare input
            var input = new RecipeInput
            {
                Text = userInput,
                AvailableRecipes = availableRecipes.Select(r => new RecipeFeatures
                {
                    Name = r.Name,
                    Tag = r.Tag,
                    Ingredients = string.Join(", ", r.Ingredients.Select(i => i.Ingredient)),
                    TotalCalories = (int)r.TotalNutrition.Calories,
                    TotalProtein = (decimal)r.TotalNutrition.Proteins,
                    TotalCarbs = (decimal)r.TotalNutrition.Carbs,
                    TotalFat = (decimal)r.TotalNutrition.Fats
                }).ToList()
            };

            // Get prediction
            var prediction = _predictionEngine.Predict(input);

            // Process results
            var recommendedRecipes = new List<RecipeJson>();
            foreach (var recipeScore in prediction.RecipeScores)
            {
                var recipe = availableRecipes.FirstOrDefault(r => r.Name == recipeScore.Name);
                if (recipe != null && recipeScore.Score > 0.5) // Threshold for recommendation
                {
                    recommendedRecipes.Add(recipe);
                }
            }

            return new RecipeRecommendationResult
            {
                Intent = prediction.Intent,
                RecommendedRecipes = recommendedRecipes.OrderByDescending(r => 
                    prediction.RecipeScores.First(s => s.Name == r.Name).Score).ToList(),
                Confidence = prediction.Confidence
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during prediction: {ex.Message}");
            throw;
        }
    }
}

public class RecipeInput
{
    [LoadColumn(0)]
    public string Text { get; set; }

    public List<RecipeFeatures> AvailableRecipes { get; set; }
}

public class RecipeFeatures
{
    public string Name { get; set; }
    public string Tag { get; set; }
    public string Ingredients { get; set; }
    public decimal TotalCalories { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalCarbs { get; set; }
    public decimal TotalFat { get; set; }
}

public class RecipePrediction
{
    [ColumnName("PredictedLabel")]
    public string Intent { get; set; }

    public float Confidence { get; set; }

    public List<RecipeScore> RecipeScores { get; set; }
}

public class RecipeScore
{
    public string Name { get; set; }
    public float Score { get; set; }
}

public class RecipeRecommendationResult
{
    public string Intent { get; set; }
    public List<RecipeJson> RecommendedRecipes { get; set; }
    public double Confidence { get; set; }
}