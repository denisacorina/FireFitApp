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


namespace RecipeRecommendation
{
    class RecipeRecommendationGen
    {
        public static void Main(string[] args)
        {
            var ingredients = File.ReadAllLines("food.csv")
             .Skip(1)
             .Select(line =>
             {
                 var parts = line.Split(',');

                 decimal TryParseDecimal(string input)
                 {
                     return decimal.TryParse(input.Trim('"'), out var value) ? value : 0;
                 }

                 return new IngredientNutrition
                 {
                     Name = parts[1].Trim('"').ToLower(),            // Clean name properly
                     Calories = TryParseDecimal(parts[11]),          // Data.Kilocalories
                     Carbs = TryParseDecimal(parts[7]),              // Data.Carbohydrate
                     Protein = TryParseDecimal(parts[17]),           // Data.Protein
                     Fat = TryParseDecimal(parts[27]),               // Data.Fat.Total Lipid
                     Fiber = TryParseDecimal(parts[10])              // Data.Fiber
                 };
             }).ToList();

            var recipes = System.Text.Json.JsonSerializer.Deserialize<List<RecipeRec>>(File.ReadAllText("recipes_combined_all_with_diet.json"));
          //  BioTagGenerator.GenerateBIO("nlp_recipe_intents_15000.json", "food.csv", "bio_annotated_dataset_old_new.json");
           // TrainModelForTextClassification();

            //PredictUserIntent();

           // var dataset = LoadBIOAnnotatedDataset("bio_annotated_dataset.json");
          //  var (X, y, word2idx, tag2idx) = PrepareData(dataset);
            //
           // var model = BuildNERModel(word2idx.Count, tag2idx.Count);

            TrainModel();
            TestPrediction();


            var ner = new NERPredictor("./ner_model", "./vocab.json");
          var service = new RecipeGeneratorService(ner, new MLModel1());

    //        var originalRecipe = new Recipe
    //        {
    //            Name = "Veggie Rice Bowl",
    //            Ingredients = new List<IngredientEntry>
    //{
    //    new() { Name = "rice", Quantity = 150, Unit = "g" },
    //    new() { Name = "broccoli", Quantity = 100, Unit = "g" },
    //    new() { Name = "olive oil", Quantity = 10, Unit = "g" }
    //}
    //        };

            string json = File.ReadAllText("C:\\Users\\z004umbe\\Downloads\\updated_recipes_with_nutrition_v2.json");

            // Deserialize the JSON into a list of Recipe objects
            var recipesJson = JsonConvert.DeserializeObject<List<RecipeJson>>(json);

            List<RecipeJson> allRecipes = recipesJson; // Assuming this is a method that retrieves all recipes
            Console.WriteLine("Enter your recipe request (e.g., 'vegan recipe under 400 kcal'):");
            string userInput = Console.ReadLine();

            // Handle the request
            var receivedRecipe = service.HandleUserRequest(userInput, allRecipes);
            //// User request: Find vegan recipes under 400 kcal
            //string dietaryPreference = "Vegan";
            //decimal maxCalories = 400;

            // Step 1: Filter the recipes by calorie limit and dietary preference
            //var filteredRecipes = service.FilterRecipes(allRecipes, maxCalories, dietaryPreference);

            //// Step 2: Adjust the recipes if they exceed the calorie limit
            //foreach (var recipe in filteredRecipes)
            //{
            //    if (recipe.TotalCalories > maxCalories)
            //    {
            //        recipe = service.AdjustRecipeToFitCalorieLimit(recipe, maxCalories);
            //    }
            //}

            Console.Write("\nWould you like to change something to the recipe? ");
            var userInputReplace = Console.ReadLine();

            var updated = service.GenerateUpdatedRecipe(userInputReplace, receivedRecipe);

            Console.WriteLine($"\n✅ Updated Recipe: {updated.Title}");
            foreach (var ing in updated.Ingredients)
                Console.WriteLine($"- {ing.Ingredient}: {ing.Quantity}{ing.Unit}");

            Console.WriteLine($"\n🔥 Calories: {updated.Calories} kcal");
            Console.WriteLine($"💪 Protein:  {updated.Protein} g");
            Console.WriteLine($"🍞 Carbs:    {updated.Carbs} g");
            Console.WriteLine($"🧈 Fat:      {updated.Fat} g");
            Console.WriteLine($"🌿 Fiber:    {updated.Fiber} g");




            //var cache = new NutritionCache();

            //IngredientNutrition GetNutrition(string name) =>
            //    cache.GetOrAdd(name, () =>
            //        ingredients.FirstOrDefault(i => i.Name == name.ToLower()));

            //Console.WriteLine($"Updated total calories: {totalCalories} kcal");

            //decimal totalCalories = recipe.Ingredients.Sum(ingredient =>
            //{
            //    var normalizedGrams = UnitNormalizer.NormalizeToGrams(ingredient.Quantity, ingredient.Unit);
            //    var nut = GetNutrition(ingredient.Name);
            //    return normalizedGrams / 100 * nut?.Calories ?? 0;
            //});
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

            var model = pipeline.Fit(data);
            var predictions = model.Transform(data);
            var metrics = mlContext.MulticlassClassification.Evaluate(predictions, labelColumnName: "Label", predictedLabelColumnName: "PredictedLabel");


            Console.WriteLine("Evaluation Metrics:");
            Console.WriteLine($"  MicroAccuracy:    {metrics.MicroAccuracy:0.###}");
            Console.WriteLine($"  MacroAccuracy:    {metrics.MacroAccuracy:0.###}");
            Console.WriteLine($"  LogLoss:          {metrics.LogLoss:0.###}");
            Console.WriteLine($"  LogLossReduction: {metrics.LogLossReduction:0.###}");

            var sentence = "replace butter with cheese";

            var testSentence = "replace sugar with stevia";
            var tokens = testSentence.Split(' ');

            var testInputs = new List<TokenInput>();
            for (int i = 0; i < tokens.Length; i++)
            {
                var prev = i > 0 ? tokens[i - 1] : "<START>";
                var curr = tokens[i];
                var next = i < tokens.Length - 1 ? tokens[i + 1] : "<END>";

                testInputs.Add(new TokenInput
                {
                    PrevToken = prev,
                    Token = curr,
                    NextToken = next
                });
            }

            var predEngine = mlContext.Model.CreatePredictionEngine<TokenInput, NerPrediction>(model);

            Console.WriteLine("Predicted tags:");
            foreach (var input in testInputs)
            {
                var prediction = predEngine.Predict(input);
                Console.WriteLine($"{input.Token,-10} => {prediction.PredictedLabel}");
            }

            mlContext.Model.Save(model, data.Schema, modelPath);
            Console.WriteLine("Model saved to: " + modelPath);
        }

        public static void PredictFromSavedModel(string sentence)
        {
            var mlContext = new MLContext();
            var modelPath = "bio_ner_model3.zip";

            // Încarcă modelul
            ITransformer loadedModel = mlContext.Model.Load(modelPath, out _);
            var predictor = mlContext.Model.CreatePredictionEngine<TokenInput, NerPrediction>(loadedModel);

            // Tokenizează propoziția
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

        public class NerPrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabel { get; set; }
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
        public static void TrainModel()
        {
            int embeddingDim = 128;
            int maxSeqLen = 100;
            int batchSize = 32;
            int numEpochs = 30;
            float learningRate = 0.001f;

            // Load and prepare data
            var dataset = LoadBIOAnnotatedDataset("bio_annotated_dataset_old_new.json");
            Console.WriteLine($"Loaded sentences: {dataset.Count}");
            var (X, y, word2idx, tag2idx) = PrepareData(dataset, maxSeqLen);
            int vocabSize = word2idx.Count;
            int tagCount = tag2idx.Count;

            // Define the Keras model
            var tf_keras = tf.keras;
            var inputs = tf_keras.Input(shape: new Shape(maxSeqLen), dtype: tf.@int32);
            var embedding = new Embedding(new EmbeddingArgs
            {
                InputDim = vocabSize,
                OutputDim = embeddingDim,
                InputLength = maxSeqLen,
                DType = tf.@float32
            });
            var x = embedding.Apply(inputs);

            var lstm = new LSTM(new LSTMArgs
            {
                Units = 64,
                ReturnSequences = true
            });

            // Define Bidirectional wrapper
            var biLstmArgs = new BidirectionalArgs
            {
                Layer = lstm,
                MergeMode = "concat", // or "sum", "ave", "mul", null
                Name = "bi_lstm"
            };

            var biLstm = new Bidirectional(biLstmArgs);

            // Apply it to input
            x = biLstm.Apply(x);
            int seqLen = maxSeqLen;
            int lstmUnits = 128; // or 64, based on previous LSTM output

            // Flatten [batch, seqLen, lstmUnits] → [batch * seqLen, lstmUnits]
            var shape = tf.shape(x);
            batchSize = (int)shape[0];
            var reshaped = tf.reshape(x, (-1, lstmUnits));

            // Apply Dense layer to each time step (same weights reused)
            var dense = tf_keras.layers.Dense(64, activation: "relu").Apply(reshaped);
            var logits = tf_keras.layers.Dense(tagCount, activation: "softmax").Apply(dense);

            // Reshape back to [batch, seqLen, tagCount]
            var outputs = tf.reshape(logits, (batchSize, seqLen, tagCount));
            var model = tf_keras.Model(inputs, outputs);
            model.compile(optimizer: tf_keras.optimizers.Adam(learningRate),
                          loss: tf.keras.losses.SparseCategoricalCrossentropy(),
                          metrics: new[] { "accuracy" });

            // Train the model
            model.fit(X, y, batch_size: batchSize, epochs: numEpochs, validation_split: 0.1f);

            // Save the model and vocabularies
            model.save("./ner_keras_model");
            SaveVocabulary(word2idx, tag2idx, "./vocab.json");

            // Example prediction
            var idx2tag = tag2idx.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            string testSentence = "replace sugar with stevia";
            var testX = PrepareTestInput(testSentence, word2idx, maxSeqLen);
            var prediction = model.predict(testX);
            var predictionNp = prediction.numpy(); // Convert to NDArray
            var predictedIndices = np.argmax(predictionNp, axis: -1).ToArray<int>();
        
            var tokens = Tokenize(testSentence);
            Console.WriteLine("Predicted tags:");
            for (int i = 0; i < tokens.Count && i < predictedIndices.Length; i++)
            {
                Console.WriteLine($"{tokens[i]}: {idx2tag[predictedIndices[i]]}");
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
            return ProcessPredictions(tokens, tags);
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
            var XArr = np.array(To2DArray(X));
            var YArr = np.array(To2DArray(Y));

            // For sparse categorical crossentropy we need y to be shaped as [samples, seqLen, 1]
            var YFinal = np.expand_dims(YArr, -1);  // [samples, maxSeqLen, 1]

            return (XArr, YFinal, word2idx, tag2idx);
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
    public string Name { get; set; }
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
    public decimal Fiber { get; set; }
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

    public string Name { get; set; }
    public string Tag { get; set; }  // For tagging recipes: Vegan, Lactose-Free, etc.
    public List<IngredientEntryJson> Ingredients { get; set; }
    public decimal TotalCalories => Ingredients.Sum(ing => NormalizeQuantity(ing.Quantity) / 100 * ing.Calories);
    public (decimal Calories, decimal Protein, decimal Carbs, decimal Fat, decimal Fiber) TotalNutrition
    {
        get
        {
            decimal kcal = 0, protein = 0, carbs = 0, fat = 0;
            foreach (var ing in Ingredients)
            {
                var factor = NormalizeQuantity(ing.Quantity) / 100m;
                kcal += (ing.Calories * factor);
                protein += (ing.Protein * factor);
                carbs += (ing.Carbs * factor);
                fat += (ing.Fat * factor);
            }
            return (kcal, protein, carbs, fat, 0); // Fiber is not calculated, placeholder set to 0
        }
    }

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