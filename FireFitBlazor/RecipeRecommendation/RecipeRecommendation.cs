using Microsoft.ML;
using Microsoft.ML.Data;

using static BioTaggedSentence;
using Newtonsoft.Json;

using Tensorflow.NumPy;
using Tensorflow;

using static Tensorflow.Binding;


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


namespace RecipeRecommendation
{
    class RecipeRecommendation
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
            //BioTagGenerator.GenerateBIO("nlp_recipe_intents_15000.json", "food.csv", "bio_annotated_dataset_old_new.json");
            // TrainModelForTextClassification();

            //PredictUserIntent();

            //var dataset = LoadBIOAnnotatedDataset("bio_annotated_dataset.json");
            //var (X, y, word2idx, tag2idx) = PrepareData(dataset);
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
        public static void TrainModel()
        {
            tf.compat.v1.disable_eager_execution();

            int embeddingDim = 128;
            int maxSeqLen = 100;
            float learningRate = 0.001f;

            // Load and prepare data
            var dataset = LoadBIOAnnotatedDataset("bio_annotated_dataset_old_new.json");
            Console.WriteLine($"Loaded sentences: {dataset.Count}");
            var (X, y, word2idx, tag2idx) = PrepareData(dataset, maxSeqLen);

            Console.WriteLine($"Original y shape: {y.shape}");
            int vocabSize = word2idx.Count;
            int tagCount = tag2idx.Count;

            // Define placeholders
            var input = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "input");
            var labels = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "labels");

            // Embedding layer
            var embeddingMatrix = tf.Variable(
                tf.random.uniform((vocabSize, embeddingDim), -1.0f, 1.0f),
                name: "embedding_matrix"
            );

            // Lookup embeddings
            var embedded = tf.nn.embedding_lookup((Tensorflow.Tensor)embeddingMatrix, input);

            // Get shape information
            var batchSize = tf.shape(input)[0];

            // Reshape for dense layer - Corrected shape handling
            var flattenedEmbedded = tf.reshape(embedded, new Shape(-1, embeddingDim));

            // Dense layer weights
            var denseWeights = tf.Variable(
                tf.random.truncated_normal((embeddingDim, tagCount), stddev: 0.1f),
                name: "dense_weights"
            );
            var denseBiases = tf.Variable(tf.zeros(tagCount), name: "dense_biases");

            // Dense layer computation
            var logits2D = tf.matmul(flattenedEmbedded, denseWeights) + denseBiases;

            // Reshape back to 3D - Corrected shape handling
            var logits = tf.reshape(logits2D, new Shape(-1, maxSeqLen, tagCount));

            // Prepare labels for loss calculation
            var flattenedLabels = tf.reshape(labels, new Shape(-1));
            var flattenedLogits = tf.reshape(logits, new Shape(-1, tagCount));

            // Calculate loss
            var loss = tf.reduce_mean(
                tf.nn.sparse_softmax_cross_entropy_with_logits(
                    labels: flattenedLabels,
                    logits: flattenedLogits
                )
            );

            // Optimizer
            var optimizer = tf.train.AdamOptimizer(learningRate);
            var trainOp = optimizer.minimize(loss);

            // Initialize session
            using var sess = tf.Session();
            sess.run(tf.global_variables_initializer());

            // Training loop
            Console.WriteLine("Starting training...");
            var feedX = X.astype(np.int32);
            var feedY = y.astype(np.int32);

            int numEpochs = 70;
            for (int epoch = 0; epoch < numEpochs; epoch++)
            {
                var (_, currentLoss) = sess.run(
                    (trainOp, loss),
                    new FeedItem(input, feedX),
                    new FeedItem(labels, feedY)
                );

                if (epoch % 1 == 0)
                {
                    Console.WriteLine($"Epoch {epoch + 1}/{numEpochs}: Loss = {currentLoss}");
                }
            }

            Console.WriteLine("Training completed!");

            // Save the model
            var saver = tf.train.Saver();
            saver.save(sess, "./ner_model");
            SaveVocabulary(word2idx, tag2idx, "./vocab.json");

            // Test prediction
            var idx2tag = tag2idx.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            string testSentence = "replace sugar with stevia";
            var prediction = PredictNER(testSentence, word2idx, idx2tag, sess, input, logits);

            Console.WriteLine($"\n🔎 Test prediction for: \"{testSentence}\"");
            Console.WriteLine($"Old Ingredient: {prediction.OldIngredient}");
            Console.WriteLine($"New Ingredient: {prediction.NewIngredient}");
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
            // Tokenize input
            var tokens = sentence.ToLower().Split(' ');
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

            // Extract entities
            string oldIngredient = "";
            string newIngredient = "";
            string currentOld = "";
            string currentNew = "";

            Console.WriteLine("\n🧠 Predicted BIO Tags:");
            for (int i = 0; i < Math.Min(tokens.Length, maxSeqLen); i++)
            {
                var tag = idx2tag[predictions[i]];
                var token = tokens[i];
                Console.WriteLine($"  {token} => {tag}");

                switch (tag)
                {
                    case "B-OLD":
                        if (!string.IsNullOrEmpty(currentOld))
                        {
                            oldIngredient = currentOld.Trim();
                            currentOld = "";
                        }
                        currentOld = token;
                        break;

                    case "I-OLD":
                        currentOld += " " + token;
                        break;

                    case "B-NEW":
                        if (!string.IsNullOrEmpty(currentNew))
                        {
                            newIngredient = currentNew.Trim();
                            currentNew = "";
                        }
                        currentNew = token;
                        break;

                    case "I-NEW":
                        currentNew += " " + token;
                        break;

                    default:
                        // Finalize current ingredients if outside tag
                        if (!string.IsNullOrEmpty(currentOld) && string.IsNullOrEmpty(oldIngredient))
                        {
                            oldIngredient = currentOld.Trim();
                            currentOld = "";
                        }
                        if (!string.IsNullOrEmpty(currentNew) && string.IsNullOrEmpty(newIngredient))
                        {
                            newIngredient = currentNew.Trim();
                            currentNew = "";
                        }
                        break;
                }
            }

            // Final fallback if still active at end
            if (!string.IsNullOrWhiteSpace(currentOld) && string.IsNullOrWhiteSpace(oldIngredient))
                oldIngredient = currentOld.Trim();

            if (!string.IsNullOrWhiteSpace(currentNew) && string.IsNullOrWhiteSpace(newIngredient))
                newIngredient = currentNew.Trim();

            return new IngredientEntities
            {
                OldIngredient = oldIngredient,
                NewIngredient = newIngredient
            };
        }




        //public static void TrainModel()
        //{
        //    tf.compat.v1.disable_eager_execution();

        //    int embeddingDim = 128;
        //    int maxSeqLen = 100;

        //    // Load and prepare data
        //    var dataset = LoadBIOAnnotatedDataset("bio_annotated_dataset.json");
        //    Console.WriteLine($"Loaded sentences: {dataset.Count}");
        //    var (X, y, word2idx, tag2idx) = PrepareData(dataset, maxSeqLen);

        //    Console.WriteLine($"Original y shape: {y.shape}");
        //    var reshapedY = y.reshape(-1);
        //    Console.WriteLine($"Flattened y shape: {reshapedY.shape}");
        //    int vocabSize = word2idx.Count;
        //    int tagCount = tag2idx.Count;

        //    // Placeholders
        //    var input = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "input");
        //    var labels = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "labels");

        //    // Embedding Layer
        //    var embeddingMatrix = tf.Variable(tf.random.uniform((vocabSize, embeddingDim), -1.0f, 1.0f), name: "embedding_matrix");
        //    var embedded = tf.nn.embedding_lookup((Tensor)embeddingMatrix, input);

        //    // Flatten and Dense
        //    var flatten = tf.reshape(embedded, (-1, maxSeqLen * embeddingDim));
        //    var weights = tf.Variable(tf.random.truncated_normal((maxSeqLen * embeddingDim, tagCount), stddev: 0.1f));

        //    var W = tf.Variable(tf.random.truncated_normal((embeddingDim, tagCount), stddev: 0.1f));
        //    var b = tf.Variable(tf.zeros(tagCount));

        //    // embedded: [batch, seq_len, embeddingDim]
        //    // reshape to 2D for matmul
        //    var embedded2D = tf.reshape(embedded, (-1, embeddingDim));          // [batch * seq_len, embDim]
        //    var logits2D = tf.matmul(embedded2D, W) + b;                         // [batch * seq_len, tagCount]
        //    var logits3D = tf.reshape(logits2D, (-1, maxSeqLen, tagCount));
        //    var biases = tf.Variable(tf.zeros(tagCount));
        //    var logits = tf.matmul(flatten, weights) + biases;
        //    logits = tf.reshape(logits, (-1, maxSeqLen, tagCount)); // [batch, seq_len, tagCount]

        //    // Reshape to [batch * seq_len, tagCount] and [batch * seq_len]
        //    var flatLogits = tf.reshape(logits3D, (-1, tagCount));
        //    var flatLabels = tf.reshape(labels, (-1));

        //    // Loss & optimizer
        //    var loss = tf.reduce_mean(tf.nn.sparse_softmax_cross_entropy_with_logits(labels: flatLabels, logits: flatLogits));
        //    var train_op = tf.train.AdamOptimizer(0.001f).minimize(loss);

        //    // Session
        //    using var sess = tf.Session();
        //    sess.run(tf.global_variables_initializer());

        //    // Use X and y as-is
        //    var feedX = X.astype(np.int32);
        //    var feedY = y.reshape(-1).astype(np.int32);
        //    var flatY = y.reshape(-1);
        //    for (int epoch = 0; epoch < 15; epoch++)
        //    {
        //        var (_, curr_loss) = sess.run((train_op, loss),
        //            new FeedItem(input, feedX),
        //            new FeedItem(labels, flatY));

        //        Console.WriteLine($"Epoch {epoch + 1}: Loss = {curr_loss}");
        //    }

        //    Console.WriteLine("✅ Model training complete.");

        //    // Save model
        //    var saver = tf.train.Saver();
        //    //saver.save(sess, "./ner_model.ckpt");

        //    var idx2tag = tag2idx.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        //    // 🔮 Predict on a sample sentence
        //    string testSentence = "replace sugar with stevia";
        //    var prediction = PredictNER(testSentence, word2idx, idx2tag, sess, input, logits);

        //    Console.WriteLine($"\n🔎 Prediction for: \"{testSentence}\"");
        //    Console.WriteLine($"Old Ingredient: {prediction.OldIngredient}");
        //    Console.WriteLine($"New Ingredient: {prediction.NewIngredient}");
        //}



        public static (NDArray X, NDArray y, Dictionary<string, int> word2idx, Dictionary<string, int> tag2idx) PrepareData(List<BioTaggedSentence> data, int maxSeqLen = 100)
        {
            var word2idx = new Dictionary<string, int> { ["<PAD>"] = 0, ["<UNK>"] = 1 };
            var tag2idx = new Dictionary<string, int> { ["O"] = 0 };

            var sequences = new List<List<int>>();
            var labels = new List<List<int>>();

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

                if (wordIds.Count == 0)
                    continue;

                // Pad
                while (wordIds.Count < maxSeqLen) wordIds.Add(0);
                while (tagIds.Count < maxSeqLen) tagIds.Add(0);

                sequences.Add(wordIds.Take(maxSeqLen).ToList());
                labels.Add(tagIds.Take(maxSeqLen).ToList());  // ✅ Add once per sentence
            }

            var X = np.array(To2DArray(sequences));
            var yArray = To2DArray(labels);

            Console.WriteLine($"✅ Final shape: {yArray.GetLength(0)} x {yArray.GetLength(1)}");
            var y = np.array(yArray);

            return (X, y, word2idx, tag2idx);
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
        //        public static (Session, Tensor inputTensor, Tensor logitsTensor, Dictionary<string, int> word2idx, Dictionary<int, string> idx2tag)
        //LoadNERModel(int maxSeqLen = 100)
        //        {
        //            tf.compat.v1.disable_eager_execution();

        //            // Rebuild your placeholders and model structure
        //            var input = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "input");
        //            var labels = tf.placeholder(tf.int32, shape: (-1, maxSeqLen), name: "labels");

        //            // You must load the same vocab and tag mappings you saved during training
        //            var dataset = LoadBIOAnnotatedDataset("bio_annotated_dataset.json");
        //            var (_, _, word2idx, tag2idx) = PrepareData(dataset, maxSeqLen);
        //            var idx2tag = tag2idx.ToDictionary(kv => kv.Value, kv => kv.Key);

        //            int vocabSize = word2idx.Count;
        //            int tagCount = tag2idx.Count;
        //            int embeddingDim = 128;

        //            // Same model structure
        //            var embeddingMatrix = tf.Variable(tf.random.uniform((vocabSize, embeddingDim), -1.0f, 1.0f), name: "embedding_matrix");
        //            var embedded = tf.nn.embedding_lookup((Tensor)embeddingMatrix, input);
        //            var flatten = tf.reshape(embedded, (-1, maxSeqLen * embeddingDim));
        //            var weights = tf.Variable(tf.random.truncated_normal((maxSeqLen * embeddingDim, tagCount), stddev: 0.1f));
        //            var biases = tf.Variable(tf.zeros(tagCount));
        //            var logits = tf.matmul(flatten, weights) + biases;
        //            logits = tf.reshape(logits, (-1, maxSeqLen, tagCount)); // Unflattened output

        //            // Restore session and weights
        //            var saver = tf.train.Saver();
        //            var sess = tf.Session();
        //            sess.run(tf.global_variables_initializer());
        //            saver.restore(sess, "./ner_model.ckpt");

        //            Console.WriteLine("✅ Model restored!");

        //            return (sess, input, logits, word2idx, idx2tag);
        //        }
        //        public static IngredientEntities PredictNER(
        //string sentence,
        //Dictionary<string, int> word2idx,
        //Dictionary<int, string> idx2tag,
        //Session session,
        //Tensor inputTensor,
        //Tensor logitsTensor,
        //int maxSeqLen = 100)
        //        {
        //            // Tokenize input
        //            var tokens = sentence.ToLower().Split(' ');
        //            var inputIds = tokens
        //                .Select(token => word2idx.TryGetValue(token, out var id) ? id : 0)
        //                .ToList();

        //            // Pad or truncate to maxSeqLen
        //            while (inputIds.Count < maxSeqLen)
        //                inputIds.Add(0);

        //            inputIds = inputIds.Take(maxSeqLen).ToList();

        //            // Prepare [1, maxSeqLen] input
        //            var input2D = new int[1, maxSeqLen];
        //            for (int i = 0; i < maxSeqLen; i++)
        //                input2D[0, i] = inputIds[i];

        //            var inputArr = np.array(input2D);
        //            Console.WriteLine($"inputArr.ndim: {inputArr.ndim}");
        //            Console.WriteLine($"inputArr.shape: {inputArr.shape}");
        //            Console.WriteLine($"inputArr.dtype: {inputArr.dtype}");
        //            Console.WriteLine($"Input shape: {inputArr.shape}");
        //            // Predict logits and argmax
        //            var logits = session.run(logitsTensor, new FeedItem(inputTensor, inputArr)); // [1, maxSeqLen, tagCount]
        //            var predictions = ((NDArray)np.argmax(logits, axis: -1)).reshape(-1).ToArray<int>(); // [maxSeqLen]

        //            // Build tagged entity strings
        //            string oldIngredient = "";
        //            string newIngredient = "";

        //            Console.WriteLine("\n🧠 Predicted BIO Tags:");
        //            for (int i = 0; i < Math.Min(tokens.Length, maxSeqLen); i++)
        //            {
        //                var tag = idx2tag[predictions[i]];
        //                Console.WriteLine($"  {tokens[i]} => {tag}");

        //                if (tag == "B-OLD" || tag == "I-OLD")
        //                    oldIngredient += (oldIngredient.Length > 0 ? " " : "") + tokens[i];
        //                else if (tag == "B-NEW" || tag == "I-NEW")
        //                    newIngredient += (newIngredient.Length > 0 ? " " : "") + tokens[i];
        //            }

        //            return new IngredientEntities
        //            {
        //                OldIngredient = string.IsNullOrWhiteSpace(oldIngredient) ? null : oldIngredient,
        //                NewIngredient = string.IsNullOrWhiteSpace(newIngredient) ? null : newIngredient
        //            };
        //        }

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
            //RestoreModel(modelPath);
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

        //private void RestoreModel(string modelPath)
        //{
        //    try
        //    {
        //        session = tf.Session();
        //        session.run(tf.global_variables_initializer());

        //        // Create saver without explicitly specifying variables
        //        var saver = tf.train.Saver();
        //        saver.restore(session, modelPath);

        //        Console.WriteLine("Model restored successfully!");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error restoring model: {ex.Message}\nStack trace: {ex.StackTrace}");
        //        throw;
        //    }
        //}

        public IngredientEntities Predict(string sentence)
        {
            try
            {
                // Tokenize and prepare input
                var tokens = sentence.ToLower().Split(' ');
                var inputIds = tokens
                    .Select(token => word2idx.TryGetValue(token, out var id) ? id : word2idx["<UNK>"])
                    .ToList();

                // Pad sequence
                while (inputIds.Count < maxSeqLen)
                    inputIds.Add(word2idx["<PAD>"]);
                inputIds = inputIds.Take(maxSeqLen).ToList();

                // Prepare input tensor
                var input2D = new int[1, maxSeqLen];
                for (int i = 0; i < maxSeqLen; i++)
                    input2D[0, i] = inputIds[i];

                var inputArr = np.array(input2D);

                // Run prediction
                //var logits = session.run(logitsTensor, new FeedItem(inputTensor, inputArr));
                //var predictions = ((NDArray)np.argmax(logits, axis: -1))
                //    .astype(np.int32)
                //    [0]
                //    .ToArray<int>();

                // Process predictions
                return ProcessPredictions(tokens);//, predictions);
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

        private IngredientEntities ProcessPredictions(string[] tokens)//, int[]? predictions)
        {
            string currentOld = "";
            string currentNew = "";
            string oldIngredient = "";
            string newIngredient = "";
            bool isSubstituteCommand = tokens.Any(t => SubstituteCommands.Contains(t));
            bool isRemoveCommand = !isSubstituteCommand && tokens.Any(t => RemoveCommands.Contains(t));
            bool isAddCommand = !isSubstituteCommand && !isRemoveCommand && tokens.Any(t => AddCommands.Contains(t));

            // For substitute commands, track state
            bool foundConnector = false;
            bool isBeforeConnector = true;

            Console.WriteLine("\nPredicted tags:");
            for (int i = 0; i < Math.Min(tokens.Length, maxSeqLen); i++)
            {
                var tag = ""; //idx2tag[predictions[i]];
                var token = tokens[i];

                if (SubstituteCommands.Contains(token) ||
                  RemoveCommands.Contains(token) ||
                  AddCommands.Contains(token))
                {
                    continue;
                }

                // Handle substitute pattern
                if (isSubstituteCommand)
                {
                    // Check for connector words
                    if (ConnectorWords.Contains(token))
                    {
                        foundConnector = true;
                        isBeforeConnector = false;
                        continue;
                    }

                    // Force correct tagging based on position relative to connector
                    if (isBeforeConnector)
                    {
                        tag = "B-OLD";
                    }
                    else if (foundConnector)
                    {
                        tag = "B-NEW";
                    }
                }
                // Handle remove/add commands
                else if (isRemoveCommand)
                {
                    tag = "B-OLD";
                }
                else if (isAddCommand)
                {
                    tag = "B-NEW";
                }

                Console.WriteLine($"{tokens[i]} => {tag}");

                switch (tag)
                {
                    case "B-OLD":
                        // Save previous if not empty
                        if (!string.IsNullOrWhiteSpace(currentOld) && string.IsNullOrWhiteSpace(oldIngredient))
                        {
                            oldIngredient = currentOld.Trim();
                        }
                        currentOld = token;
                        break;

                    case "I-OLD":
                        currentOld += " " + token;
                        break;

                    case "B-NEW":
                        if (!string.IsNullOrWhiteSpace(currentNew) && string.IsNullOrWhiteSpace(newIngredient))
                        {
                            newIngredient = currentNew.Trim();
                        }
                        currentNew = token;
                        break;

                    case "I-NEW":
                        currentNew += " " + token;
                        break;

                    default:
                        // Finalize if hitting an O or new tag type
                        if (!string.IsNullOrWhiteSpace(currentOld) && string.IsNullOrWhiteSpace(oldIngredient))
                        {
                            oldIngredient = currentOld.Trim();
                            currentOld = "";
                        }
                        if (!string.IsNullOrWhiteSpace(currentNew) && string.IsNullOrWhiteSpace(newIngredient))
                        {
                            newIngredient = currentNew.Trim();
                            currentNew = "";
                        }
                        break;
                }
            }

            // Final fallback if we ended on an ingredient
            if (!string.IsNullOrWhiteSpace(currentOld) && string.IsNullOrWhiteSpace(oldIngredient))
                oldIngredient = currentOld.Trim();

            if (!string.IsNullOrWhiteSpace(currentNew) && string.IsNullOrWhiteSpace(newIngredient))
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