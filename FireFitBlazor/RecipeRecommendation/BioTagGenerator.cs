using System.Text.Json;
using System.Text.RegularExpressions;
using CsvHelper;
using System.Globalization;
using System.Text;
using System.Formats.Asn1;
using CsvHelper.Configuration.Attributes;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using static Tensorflow.TensorSliceProto.Types;

//public class BioTaggedToken
//{
//    public string Token { get; set; }
//    public string Tag { get; set; }
//}

//public class BioTaggedSentence
//{
//    public List<BioTaggedToken> Tokens { get; set; } = new();

//    public static class BioTagGenerator
//    {
//        public class NerExample
//        {
//            public string Text { get; set; }
//            public string Intent { get; set; }
//        }

//        // 2) change GenerateBIO to load NerExample instead of just sentences
//        public static void GenerateBIO(string jsonPath, string csvPath, string outputPath)
//        {
//            var ingredients = LoadIngredients(csvPath); // ingredient list
//            var examples = LoadExamples(jsonPath);      // text + intent

//            var tagged = examples
//                .Select(x => TokenizeAndTag(x.Text, x.Intent, ingredients))
//                .ToList();  // 1 BioTaggedSentence per input

//            var json = JsonSerializer.Serialize(tagged, new JsonSerializerOptions { WriteIndented = true });
//            File.WriteAllText(outputPath, json);

//            Console.WriteLine($"✅ Saved {tagged.Count} annotated sentences to {outputPath}");
//        }

//        // 3) a new loader that gives you both text and intent
//        static List<NerExample> LoadExamples(string jsonPath)
//        {
//            var text = File.ReadAllText(jsonPath);
//            return JsonSerializer
//              .Deserialize<List<NerExample>>(text, new JsonSerializerOptions
//              {
//                  PropertyNameCaseInsensitive = true
//              });
//        }

//        // (no change needed to LoadIngredients)
//        static List<string> LoadIngredients(string csvPath)
//        {
//            using var reader = new StreamReader(csvPath);
//            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

//            var rows = csv.GetRecords<FoodCsvEntry>().ToList();
//            return rows
//              .Select(r => r.Category?.Trim().ToLowerInvariant())
//              .Where(n => !string.IsNullOrEmpty(n))
//              .Distinct()
//              .ToList();
//        }

//        public static BioTaggedSentence TokenizeAndTag(
//        string sentence,
//        string intent,
//        List<string> knownIngredients         // optional, for late fallback
//    )
//        {
//            // 1) tokenize exactly as before
//            var tokens = Regex.Matches(sentence, @"\w+|[^\w\s]")
//                              .Select(m => new BioTaggedToken { Token = m.Value })
//                              .ToList();

//            var tags = Enumerable.Repeat("O", tokens.Count).ToArray();

//            // helpers to mark a contiguous phrase in the token list
//            static void TagPhrase(List<BioTaggedToken> toks, string[] phraseTokens, string[] tagArray)
//            {
//                for (int i = 0; i <= toks.Count - phraseTokens.Length; i++)
//                {
//                    bool match = true;
//                    for (int j = 0; j < phraseTokens.Length; j++)
//                    {
//                        if (!string.Equals(
//                                toks[i + j].Token,
//                                phraseTokens[j],
//                                StringComparison.OrdinalIgnoreCase
//                             ))
//                        {
//                            match = false;
//                            break;
//                        }
//                    }

//                    if (match)
//                    {
//                        tagArray[i] = "B-ING";
//                        for (int j = 1; j < phraseTokens.Length; j++)
//                            tagArray[i + j] = "I-ING";
//                        return;   // only first occurrence
//                    }
//                }
//            }

//            // 2) pattern-based pull
//            //    for SUBSTITUTION we expect: "use A instead of B" | "swap A for B" | "replace A with B"
//            if (intent == "substitute")
//            {
//                var subRx = new Regex(
//   @"\b(?:(?:use|swap|switch|trade|replace|exchange|substitute|change|prefer))\s+"    // verb
// + @"(.+?)\s+"                                                                         // group 1 = new
// + @"(?:(?:instead\s+of)|for|with|to|over)\s+"                                         // connector
// + @"(.+?)\b",                                                                        // group 2 = old
//   RegexOptions.IgnoreCase);

//                var preferRx = new Regex(@"\bprefer\s+(.+?)\s+over\s+(.+?)\b", RegexOptions.IgnoreCase);
//                var m = subRx.Match(sentence);
//                if (!m.Success) m = preferRx.Match(sentence);
//                if (m.Success)
//                {
//                    var newIng = m.Groups[1].Value.Trim();
//                    var oldIng = m.Groups[2].Value.Trim();

//                    TagPhrase(tokens, newIng.Split(' '), tags);
//                    TagPhrase(tokens, oldIng.Split(' '), tags);
//                }
//            }
//            //  for ADD we expect: "add X" | "include X" | "insert X" | "throw in X"
//            else if (intent == "add")
//            {
//                var addRx = new Regex(
//  @"\b(?:(?:add|include|insert|throw in|put|let'?s put|could you add|please (?:add|include)|i(?:'d| would)? like to (?:add|include)|i want to include))\s+(.+?)\b",
//  RegexOptions.IgnoreCase);
//                var m = addRx.Match(sentence);
//                if (m.Success)
//                    TagPhrase(tokens, m.Groups[1].Value.Trim().Split(' '), tags);
//            }
//            //  for REMOVE we expect: "remove X" | "skip X" | "delete X" | "exclude X" | "get rid of X"
//            else if (intent == "remove")
//            {
//                var remRx = new Regex(
//   @"\b(?:(?:remove|skip|delete|exclude|get rid of|take out|omit|avoid|leave out|don't use|no))\s+(.+?)\b",
//   RegexOptions.IgnoreCase);
//                var m = remRx.Match(sentence);
//                if (m.Success)
//                    TagPhrase(tokens, m.Groups[1].Value.Trim().Split(' '), tags);
//            }

//            // 3) fallback: if you still have a known‐ingredient list, n-gram scan it *only over O’s*
//            if (knownIngredients?.Count > 0)
//            {
//                var lowerKnown = new HashSet<string>(
//                    knownIngredients
//                      .Select(i => i.ToLowerInvariant().Trim())
//                );

//                var lowerTokens = tokens.Select(t => t.Token.ToLowerInvariant()).ToList();

//                // longest 4-gram → 1-gram
//                for (int n = 4; n >= 1; n--)
//                {
//                    for (int i = 0; i + n <= tokens.Count; i++)
//                    {
//                        if (tags[i] != "O") continue;
//                        var phrase = string.Join(" ", lowerTokens.Skip(i).Take(n)).Trim();
//                        if (lowerKnown.Contains(phrase))
//                        {
//                            tags[i] = "B-ING";
//                            for (int j = 1; j < n; j++)
//                                tags[i + j] = "I-ING";
//                        }

//                    }
//                }
//            }
//            for (int i = 0; i < tokens.Count; i++)
//            {
//                tokens[i].Tag = tags[i];
//            }
//            return new BioTaggedSentence
//            {
//                Tokens = tokens,
//            };
//        }



//        public class FoodCsvEntry
//        {
//            public string Category { get; set; }
//            public string Description { get; set; }

//            [Name("Data.Carbohydrate")]
//            public decimal Carbohydrate { get; set; }

//            [Name("Data.Fiber")]
//            public decimal Fiber { get; set; }

//            [Name("Data.Kilocalories")]
//            public decimal Kilocalories { get; set; }

//            [Name("Data.Fat.Saturated Fat")]
//            public decimal SaturatedFat { get; set; }

//            [Name("Data.Protein")]
//            public decimal Protein { get; set; }
//        }

//        // Token-to-ID dictionary builder
//        public static Dictionary<string, int> BuildVocabulary(List<BioTaggedSentence> data)
//        {
//            var vocab = new Dictionary<string, int> { ["<PAD>"] = 0, ["<UNK>"] = 1 };
//            foreach (var sentence in data)
//            {
//                foreach (var token in sentence.Tokens)
//                {
//                    var word = token.Token.ToLowerInvariant();
//                    if (!vocab.ContainsKey(word))
//                        vocab[word] = vocab.Count;
//                }
//            }
//            return vocab;
//        }

//        public static Dictionary<string, int> BuildTagMap(List<BioTaggedSentence> data)
//        {
//            var tags = new HashSet<string>(data.SelectMany(s => s.Tokens.Select(t => t.Tag).ToList()));
//            return tags.Select((tag, idx) => new { tag, idx }).ToDictionary(x => x.tag, x => x.idx);
//        }

//        public class BioNERExample
//        {
//            public List<string> Tokens { get; set; }
//            public List<string> Tags { get; set; }
//        }

//        public static List<BioNERExample> LoadBIOData(string path)
//        {
//            var raw = File.ReadAllText(path);
//            var rawList = JsonSerializer.Deserialize<List<BioTaggedSentence>>(raw);
//            return rawList.Select(entry => new BioNERExample
//            {
//                Tokens = entry.Tokens.Select(t => t.Token).ToList(),
//                Tags = entry.Tokens.Select(t => t.Tag).ToList(),
//            }).ToList();
//        }

//        public class Vocab
//        {
//            public Dictionary<string, int> TokenToId { get; set; } = new();
//            public Dictionary<string, int> TagToId { get; set; } = new();

//            public void Build(List<BioNERExample> examples)
//            {
//                var tokens = examples.SelectMany(e => e.Tokens).Distinct();
//                var tags = examples.SelectMany(e => e.Tags).Distinct();

//                int i = 1; // 0 reserved for padding
//                foreach (var token in tokens) TokenToId[token.ToLower()] = i++;
//                i = 0;
//                foreach (var tag in tags) TagToId[tag] = i++;
//            }
//        }

//        public class PreprocessedNERData
//        {
//            public int[][] TokenIds;
//            public int[][] TagIds;
//            public Dictionary<string, int> WordToIdx;
//            public Dictionary<string, int> TagToIdx;
//            public Dictionary<int, string> IdxToTag;
//        }

//        public static PreprocessedNERData LoadAndEncodeNERDataset(string jsonPath, int maxSeqLength = 30)
//        {
//            var json = File.ReadAllText(jsonPath);
//            var sentences = JsonSerializer.Deserialize<List<BioTaggedSentence>>(json);

//            var wordToIdx = new Dictionary<string, int> { ["<PAD>"] = 0, ["<UNK>"] = 1 };
//            var tagToIdx = new Dictionary<string, int> { ["O"] = 0 };
//            var idxToTag = new Dictionary<int, string> { [0] = "O" };

//            int wordIndex = 2;
//            int tagIndex = 1;

//            var tokenIds = new List<int[]>();
//            var tagIds = new List<int[]>();

//            foreach (var sentence in sentences)
//            {
//                var tokens = new List<int>();
//                var tags = new List<int>();

//                foreach (var token in sentence.Tokens.Select(t => t.Token.ToLowerInvariant()))
//                {
//                    if (!wordToIdx.ContainsKey(token))
//                        wordToIdx[token] = wordIndex++;

//                    tokens.Add(wordToIdx[token]);
//                }

//                foreach (var tag in sentence.Tokens.Select(t => t.Tag).ToList())
//                {
//                    if (!tagToIdx.ContainsKey(tag))
//                    {
//                        tagToIdx[tag] = tagIndex;
//                        idxToTag[tagIndex] = tag;
//                        tagIndex++;
//                    }

//                    tags.Add(tagToIdx[tag]);
//                }

//                // Pad to max length
//                if (tokens.Count < maxSeqLength)
//                {
//                    tokens.AddRange(Enumerable.Repeat(0, maxSeqLength - tokens.Count));
//                    tags.AddRange(Enumerable.Repeat(0, maxSeqLength - tags.Count));
//                }
//                else
//                {
//                    tokens = tokens.Take(maxSeqLength).ToList();
//                    tags = tags.Take(maxSeqLength).ToList();
//                }

//                tokenIds.Add(tokens.ToArray());
//                tagIds.Add(tags.ToArray());
//            }

//            return new PreprocessedNERData
//            {
//                TokenIds = tokenIds.ToArray(),
//                TagIds = tagIds.ToArray(),
//                WordToIdx = wordToIdx,
//                TagToIdx = tagToIdx,
//                IdxToTag = idxToTag
//            };
//        }

//    }
//}

public class BioTaggedToken
{
    public string Token { get; set; }
    public string Tag { get; set; }
}

public class BioTaggedSentence
{
    public List<BioTaggedToken> Tokens { get; set; } = new();

    public static class BioTagGenerator
    {
        private static readonly HashSet<string> CompoundIngredients = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    // Dairy and Alternatives
    "almond milk",
    "coconut milk",
    "soy milk",
    "oat milk",
    "vegan cheese",
    "cream cheese",
    "greek yogurt",
    
    // Oils and Fats
    "olive oil",
    "coconut oil",
    
    // Condiments and Seasonings
    "apple cider vinegar",
    "soy sauce",
    "maple syrup",
    "peanut butter",
    "vanilla extract",
    
    // Flours and Grains
    "almond flour",
    "coconut flour",
    "oat flour",
    "whole wheat",
    "brown rice",
    
    // Sweeteners
    "brown sugar",
    "maple sugar",
    
    // Baking Items
    "baking powder",
    "baking soda",
    
    // Beverages/Cooking Liquids
    "apple cider",
    "red wine",
    "white wine"
};

        public class NerExample
        {
            public string Text { get; set; }
            public string Intent { get; set; }
        }

        public static void GenerateBIO(string jsonPath, string csvPath, string outputPath)
        {
            //var ingredients = LoadIngredients(csvPath);
            var examples = LoadExamples(jsonPath);

            //Console.WriteLine($"Loaded {ingredients.Count} ingredients");
            Console.WriteLine($"Loaded {examples.Count} examples");

            var tagged = examples
                .Select(x =>
                {
                    var result = TokenizeAndTag(x.Text, x.Intent);
                    if (!result.Tokens.Any(t => t.Tag.StartsWith("B-ING")))
                    {
                        Console.WriteLine($"Warning: No ingredients found in: {x.Text}");
                    }
                    return result;
                })
                .ToList();

            // Validate tag distribution
            var tagCounts = tagged
                .SelectMany(s => s.Tokens)
                .GroupBy(t => t.Tag)
                .ToDictionary(g => g.Key, g => g.Count());

            Console.WriteLine("\nTag distribution:");
            foreach (var kvp in tagCounts)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            var json = JsonSerializer.Serialize(tagged, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            File.WriteAllText(outputPath, json);
            Console.WriteLine($"✅ Saved {tagged.Count} annotated sentences to {outputPath}");
        }

        static List<NerExample> LoadExamples(string jsonPath)
        {
            var text = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<List<NerExample>>(text, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        static List<string> LoadIngredients(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var rows = csv.GetRecords<FoodCsvEntry>().ToList();
            var ingredients = rows
                .Select(r => r.Category?.Trim().ToLowerInvariant())
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .ToList();

            // Add compound ingredients to the list
            ingredients.AddRange(CompoundIngredients.Select(c => c.ToLowerInvariant()));

            return ingredients.Distinct().ToList();
        }

        private static string CleanText(string text)
        {
            // Remove extra whitespace
            text = Regex.Replace(text.Trim(), @"\s+", " ");
            // Remove special characters except basic punctuation
            text = Regex.Replace(text, @"[^a-zA-Z0-9\s.,!?-]", "");
            return text;
        }

        public static BioTaggedSentence TokenizeAndTag(
    string sentence,
    string intent
   )
        {
            sentence = CleanText(sentence);

            // Enhanced patterns with more variations
            var patterns = new Dictionary<string, (string pattern, string[] groups)>
    {
        {
            "substitute",
            (
                @"\b(?:use|swap|switch|trade|replace|exchange|substitute|change|prefer|can\s+(?:we|you)\s+swap)\s+" +
                @"((?:[\w\s-]+?))\s+" +
                @"(?:instead\s+of|for|with|to|over)\s+" +
                @"((?:[\w\s-]+?))\b",
                new[] { "new", "old" }
            )
        },
        {
            "add",
            (
                @"\b(?:add|include|insert|throw\s+in|put|let'?s\s+put|could\s+you\s+add|please\s+(?:add|include)|" +
                @"i(?:'d| would)?\s+like\s+to\s+(?:add|include)|i\s+want\s+to\s+include|let'?s|add\s+to\s+the\s+list|" +
                @"throw\s+in\s+some|include\s+please)\s+" +
                @"((?:[\w\s-]+?))\b",
                new[] { "new" }
            )
        },
        {
            "remove",
            (
                @"\b(?:remove|skip|delete|exclude|get\s+rid\s+of|take\s+out|omit|avoid|leave\s+out|don'?t\s+use|no|eliminate|" +
                @"delete\s+from\s+the\s+list|can\s+you\s+remove)\s+" +
                @"((?:[\w\s-]+?))\b|" +
                @"no\s+((?:[\w\s-]+?))\s+in\s+this",
                new[] { "old" }
            )
        }
    };

            // First, check for compound ingredients
            var compoundMatches = new List<(int Start, int End, string Ingredient)>();
            //foreach (var compound in CompoundIngredients.Concat(knownIngredients))
            //{
            //    var regex = new Regex($@"\b{Regex.Escape(compound)}\b", RegexOptions.IgnoreCase);
            //    var matches = regex.Matches(sentence);
            //    foreach (Match match in matches)
            //    {
            //        compoundMatches.Add((match.Index, match.Index + match.Length, match.Value));
            //    }
            //}

            // Sort matches by start position
            compoundMatches = compoundMatches.OrderBy(m => m.Start).ToList();

            // Tokenize while preserving compound ingredients
            var tokens = new List<BioTaggedToken>();
            var currentPos = 0;

            foreach (var match in compoundMatches)
            {
                if (currentPos < match.Start)
                {
                    var beforeText = sentence.Substring(currentPos, match.Start - currentPos);
                    tokens.AddRange(TokenizeText(beforeText));
                }

                tokens.Add(new BioTaggedToken { Token = match.Ingredient });
                currentPos = match.End;
            }

            if (currentPos < sentence.Length)
            {
                var remainingText = sentence.Substring(currentPos);
                tokens.AddRange(TokenizeText(remainingText));
            }

            if (tokens.Count == 0)
            {
                tokens = TokenizeText(sentence);
            }

            var tags = Enumerable.Repeat("O", tokens.Count).ToArray();

            // Apply intent-based tagging
            if (patterns.TryGetValue(intent, out var intentPattern))
            {
                var regex = new Regex(intentPattern.pattern, RegexOptions.IgnoreCase);
                var match = regex.Match(sentence);

                if (match.Success)
                {
                    for (int i = 0; i < intentPattern.groups.Length; i++)
                    {
                        var groupIndex = i + 1;
                        if (match.Groups.Count > groupIndex && match.Groups[groupIndex].Success)
                        {
                            var ingredient = match.Groups[groupIndex].Value.Trim();
                            TagPhrase(tokens, ingredient.Split(' '), tags, intent, intentPattern.groups[i]);
                        }
                    }
                }
            }

            // Apply tags to tokens and validate
            for (int i = 0; i < tokens.Count; i++)
            {
                tokens[i].Tag = tags[i];
            }

            ValidateTagging(tokens);

            var hasIngredients = tokens.Any(t => t.Tag.EndsWith("-OLD") || t.Tag.EndsWith("-NEW"));
            if (!hasIngredients)
            {
                // Try one more time with simple ingredient matching
                for (int i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i].Token.ToLowerInvariant();
                    if /*(knownIngredients.Contains(token, StringComparer.OrdinalIgnoreCase) ||*/
                        (CompoundIngredients.Contains(token, StringComparer.OrdinalIgnoreCase))
                    {
                        string prefix = intent switch
                        {
                            "remove" => "OLD",
                            "add" => "NEW",
                            "substitute" => "NEW",
                            _ => "NEW"
                        };
                        tokens[i].Tag = $"B-{prefix}";
                    }
                }
            }

            return new BioTaggedSentence { Tokens = tokens };
        }

        private static void ValidateTagging(List<BioTaggedToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if ((tokens[i].Tag == "I-OLD" || tokens[i].Tag == "I-NEW") &&
                    (i == 0 || tokens[i - 1].Tag == "O"))
                {
                    tokens[i].Tag = tokens[i].Tag.Replace("I", "B");
                }
            }
        }

        private static List<BioTaggedToken> TokenizeText(string text)
        {
            return Regex.Matches(text, @"\b\w+\b|[^\w\s]")
                .Select(m => new BioTaggedToken { Token = m.Value })
                .ToList();
        }

        private static void TagPhrase(List<BioTaggedToken> tokens, string[] phraseTokens, string[] tagArray, string intent, string target)
        {
            for (int i = 0; i <= tokens.Count - phraseTokens.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < phraseTokens.Length; j++)
                {
                    // Check for compound ingredients first
                    var phraseJoined = string.Join(" ", phraseTokens);
                    if (CompoundIngredients.Contains(phraseJoined, StringComparer.OrdinalIgnoreCase))
                    {
                        // If it's a compound ingredient, match it as a whole
                        if (!string.Equals(string.Join(" ", tokens.Skip(i).Take(phraseTokens.Length).Select(t => t.Token)),
                            phraseJoined, StringComparison.OrdinalIgnoreCase))
                        {
                            match = false;
                            break;
                        }
                    }
                    else if (!string.Equals(tokens[i + j].Token, phraseTokens[j], StringComparison.OrdinalIgnoreCase))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    string tagPrefix = intent switch
                    {
                        "substitute" => target == "new" ? "NEW" : "OLD",
                        "add" => "NEW",
                        "remove" => "OLD",
                        _ => "O"
                    };

                    tagArray[i] = $"B-{tagPrefix}";
                    for (int j = 1; j < phraseTokens.Length; j++)
                        tagArray[i + j] = $"I-{tagPrefix}";

                    return;
                }
            }
        }
        public static void LoadCompoundIngredients(string filePath)
        {
            if (File.Exists(filePath))
            {
                var compounds = File.ReadAllLines(filePath)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Trim().ToLowerInvariant());

                foreach (var compound in compounds)
                {
                    CompoundIngredients.Add(compound);
                }
            }
        }

        public class FoodCsvEntry
        {
            public string Category { get; set; }
            public string Description { get; set; }

            [Name("Data.Carbohydrate")]
            public decimal Carbohydrate { get; set; }

            [Name("Data.Fiber")]
            public decimal Fiber { get; set; }

            [Name("Data.Kilocalories")]
            public decimal Kilocalories { get; set; }

            [Name("Data.Fat.Saturated Fat")]
            public decimal SaturatedFat { get; set; }

            [Name("Data.Protein")]
            public decimal Protein { get; set; }
        }

        // Token-to-ID dictionary builder
        public static Dictionary<string, int> BuildVocabulary(List<BioTaggedSentence> data)
        {
            var vocab = new Dictionary<string, int> { ["<PAD>"] = 0, ["<UNK>"] = 1 };
            foreach (var sentence in data)
            {
                foreach (var token in sentence.Tokens)
                {
                    var word = token.Token.ToLowerInvariant();
                    if (!vocab.ContainsKey(word))
                        vocab[word] = vocab.Count;
                }
            }
            return vocab;
        }

        public static Dictionary<string, int> BuildTagMap(List<BioTaggedSentence> data)
        {
            var tags = new HashSet<string>(data.SelectMany(s => s.Tokens.Select(t => t.Tag)));
            return tags.Select((tag, idx) => new { tag, idx }).ToDictionary(x => x.tag, x => x.idx);
        }

        public class BioNERExample
        {
            public List<string> Tokens { get; set; }
            public List<string> Tags { get; set; }
        }

        public static List<BioNERExample> LoadBIOData(string path)
        {
            var raw = File.ReadAllText(path);
            var rawList = JsonSerializer.Deserialize<List<BioTaggedSentence>>(raw);
            return rawList.Select(entry => new BioNERExample
            {
                Tokens = entry.Tokens.Select(t => t.Token).ToList(),
                Tags = entry.Tokens.Select(t => t.Tag).ToList(),
            }).ToList();
        }

        public class Vocab
        {
            public Dictionary<string, int> TokenToId { get; set; } = new();
            public Dictionary<string, int> TagToId { get; set; } = new();

            public void Build(List<BioNERExample> examples)
            {
                // Add special tokens first
                TokenToId["<PAD>"] = 0;
                TokenToId["<UNK>"] = 1;

                var tokens = examples.SelectMany(e => e.Tokens).Distinct();
                var tags = examples.SelectMany(e => e.Tags).Distinct();

                int i = TokenToId.Count; // Start after special tokens
                foreach (var token in tokens)
                {
                    if (!TokenToId.ContainsKey(token.ToLower()))
                    {
                        TokenToId[token.ToLower()] = i++;
                    }
                }

                i = 0; // Tags start from 0
                foreach (var tag in tags)
                {
                    if (!TagToId.ContainsKey(tag))
                    {
                        TagToId[tag] = i++;
                    }
                }
            }
        }

        public class PreprocessedNERData
        {
            public int[][] TokenIds { get; set; }
            public int[][] TagIds { get; set; }
            public Dictionary<string, int> WordToIdx { get; set; }
            public Dictionary<string, int> TagToIdx { get; set; }
            public Dictionary<int, string> IdxToTag { get; set; }
        }

        public static PreprocessedNERData LoadAndEncodeNERDataset(string jsonPath, int maxSeqLength = 30)
        {
            try
            {
                var json = File.ReadAllText(jsonPath);
                var sentences = JsonSerializer.Deserialize<List<BioTaggedSentence>>(json);

                var wordToIdx = new Dictionary<string, int> { ["<PAD>"] = 0, ["<UNK>"] = 1 };
                var tagToIdx = new Dictionary<string, int> { ["O"] = 0 };
                var idxToTag = new Dictionary<int, string> { [0] = "O" };

                int wordIndex = 2; // Start after special tokens
                int tagIndex = 1;

                var tokenIds = new List<int[]>();
                var tagIds = new List<int[]>();

                foreach (var sentence in sentences)
                {
                    var tokens = new List<int>();
                    var tags = new List<int>();

                    foreach (var token in sentence.Tokens)
                    {
                        var word = token.Token.ToLowerInvariant();
                        if (!wordToIdx.ContainsKey(word))
                            wordToIdx[word] = wordIndex++;

                        if (!tagToIdx.ContainsKey(token.Tag))
                        {
                            tagToIdx[token.Tag] = tagIndex;
                            idxToTag[tagIndex] = token.Tag;
                            tagIndex++;
                        }

                        tokens.Add(wordToIdx[word]);
                        tags.Add(tagToIdx[token.Tag]);
                    }

                    // Pad sequences
                    if (tokens.Count < maxSeqLength)
                    {
                        var padLength = maxSeqLength - tokens.Count;
                        tokens.AddRange(Enumerable.Repeat(0, padLength));
                        tags.AddRange(Enumerable.Repeat(0, padLength));
                    }
                    else
                    {
                        tokens = tokens.Take(maxSeqLength).ToList();
                        tags = tags.Take(maxSeqLength).ToList();
                    }

                    tokenIds.Add(tokens.ToArray());
                    tagIds.Add(tags.ToArray());
                }

                return new PreprocessedNERData
                {
                    TokenIds = tokenIds.ToArray(),
                    TagIds = tagIds.ToArray(),
                    WordToIdx = wordToIdx,
                    TagToIdx = tagToIdx,
                    IdxToTag = idxToTag
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing NER dataset: {ex.Message}");
                throw;
            }
        }
    }
}