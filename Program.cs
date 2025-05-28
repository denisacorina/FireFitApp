using System;
using System.Collections.Generic;
using System.Linq;
using RecipeLLM;
using System.Diagnostics;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;
using static test.LayerNorm;
using static test.Linear;

namespace test;

public static class MatrixUtils
{
    public static double[][] Random(int rows, int cols)
    {
        Random rnd = new();
        return Enumerable.Range(0, rows)
            .Select(_ => Enumerable.Range(0, cols).Select(_ => rnd.NextDouble() * 0.02 - 0.01).ToArray())
            .ToArray();
    }

    public static double[] Softmax(double[] x)
    {
        double max = x.Max();
        var exps = x.Select(v => Math.Exp(Math.Min(v - max, 30))).ToArray();
        double sum = exps.Sum();
        return exps.Select(v => v / sum).ToArray();
    }

    public static double Dot(double[] a, double[] b)
    {
        double sum = 0;
        for (int i = 0; i < a.Length; i++) sum += a[i] * b[i];
        return sum;
    }

    public static double[] Add(double[] a, double[] b)
    {
        return a.Zip(b, (x, y) => x + y).ToArray();
    }

    public static double[][] Add(double[][] a, double[][] b)
    {
        return a.Zip(b, (rowA, rowB) => Add(rowA, rowB)).ToArray();
    }

    public static double[] ReLU(double[] x)
    {
        return x.Select(v => Math.Max(0, v)).ToArray();
    }

    public static double[] SoftmaxBackward(double[] softmaxOutput, double[] gradOutput)
    {
        int n = softmaxOutput.Length;
        double[] gradInput = new double[n];

        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int j = 0; j < n; j++)
            {
                double delta = i == j ? 1.0 : 0.0;
                double jacobian = softmaxOutput[i] * (delta - softmaxOutput[j]);
                sum += gradOutput[j] * jacobian;
            }
            gradInput[i] = sum;
        }

        return gradInput;
    }
}

public static float[][] Random(int rows, int cols)
{
    Random rnd = new();
    return Enumerable.Range(0, rows)
        .Select(_ => Enumerable.Range(0, cols).Select(_ => (float)(rnd.NextDouble() * 0.02 - 0.01)).ToArray())
        .ToArray();
}

public static float[] Softmax(float[] x)
{
    float max = x.Max();
    var exps = x.Select(v => (float)Math.Exp(Math.Min(v - max, 30))).ToArray();
    float sum = exps.Sum();
    return exps.Select(v => v / sum).ToArray();
}

public static float Dot(float[] a, float[] b)
{
    float sum = 0;
    for (int i = 0; i < a.Length; i++) sum += a[i] * b[i];
    return sum;
}

public static float[] Add(float[] a, float[] b)
{
    return a.Zip(b, (x, y) => x + y).ToArray();
}

public static float[][] Add(float[][] a, float[][] b)
{
    return a.Zip(b, (rowA, rowB) => Add(rowA, rowB)).ToArray();
}

public static float[] ReLU(float[] x)
{
    return x.Select(v => Math.Max(0, v)).ToArray();
}

public static float[] SoftmaxBackward(float[] softmaxOutput, float[] gradOutput)
{
    int n = softmaxOutput.Length;
    float[] gradInput = new float[n];

    for (int i = 0; i < n; i++)
    {
        float sum = 0;
        for (int j = 0; j < n; j++)
        {
            float delta = i == j ? 1.0f : 0.0f;
            float jacobian = softmaxOutput[i] * (delta - softmaxOutput[j]);
            sum += gradOutput[j] * jacobian;
        }
        gradInput[i] = sum;
    }

    return gradInput;
}

public class LayerNorm
{
    public int Dim;
    private float[] mean, std;
    private float[][] lastInput;
    private float[][] xNorm;

    public LayerNorm(int dim)
    {
        Dim = dim;
    }

    public float[] Forward(float[] x)
    {
        float mean = x.Average();
        float variance = x.Select(v => (v - mean) * (v - mean)).Average();
        float std = (float)Math.Sqrt(variance + 1e-5f);
        return x.Select(v => (v - mean) / std).ToArray();
    }

    public float[][] Forward(float[][] input)
    {
        int batchSize = input.Length;
        lastInput = new float[batchSize][];
        xNorm = new float[batchSize][];
        mean = new float[batchSize];
        std = new float[batchSize];

        for (int i = 0; i < batchSize; i++)
        {
            var x = input[i];
            lastInput[i] = x;

            mean[i] = x.Average();
            float variance = x.Select(v => (v - mean[i]) * (v - mean[i])).Average();
            std[i] = (float)Math.Sqrt(variance + 1e-5f);

            xNorm[i] = x.Select(v => (v - mean[i]) / std[i]).ToArray();
        }

        return xNorm;
    }
}

public class AdamOptimizer
{
    public readonly float learningRate, beta1, beta2, epsilon, weightDecay;
    public Dictionary<(int, int), float> m = new();
    public Dictionary<(int, int), float> v = new();
    public Dictionary<(string, int), float> mm = new();
    public Dictionary<(string, int), float> vv = new();
    public int timestep = 0;

    public AdamOptimizer(float lr = 0.01f, float b1 = 0.9f, float b2 = 0.999f, float eps = 1e-8f, float wd = 0.01f)
    {
        learningRate = lr;
        beta1 = b1;
        beta2 = b2;
        epsilon = eps;
        weightDecay = wd;
    }
}

public class Linear
{
    public float[][] Weights;
    public float[] Biases;
    private int inDim, outDim;
    private AdamOptimizer optimizer;
    private float[][] lastInput;
    private float clip;
    public float[][] GradWeights;
    public float[] GradBiases;

    public Linear(int inDim, int outDim, AdamOptimizer optimizer, float clip = 5.0f)
    {
        this.inDim = inDim;
        this.outDim = outDim;
        this.optimizer = optimizer;
        this.clip = clip;

        Weights = MatrixUtils.Random(inDim, outDim);
        Biases = new float[outDim];

        GradWeights = new float[inDim][];
        for (int i = 0; i < inDim; i++)
            GradWeights[i] = new float[outDim];

        GradBiases = new float[outDim];
    }
}

public static class Dropout
{
    private static Random rand = new();

    public static float[] Apply(float[] input, float dropoutRate)
    {
        if (dropoutRate <= 0) return input;
        return input.Select(v => rand.NextDouble() < dropoutRate ? 0.0f : v).ToArray();
    }

    public static float[][] Apply(float[][] input, float dropoutRate)
    {
        return input.Select(row => Apply(row, dropoutRate)).ToArray();
    }
}

public class GPTModel
{
    public int blockSize, embedDim, vocabSize;
    public Linear tokenEmbedLinear;
    public Linear positionEmbedLinear;
    public float[][] tokenEmbeddings;
    public float[][] positionEmbeddings;

    public float[][] tokenEmbeddingGrads;
    public float[][] positionEmbeddingGrads;

    public List<TransformerBlock> transformerBlocks;
    public Linear lmHead;

    private List<TransformerBlock> blocks;
    private LayerNorm norm;

    private float[][] lastTokenEmbeddings;
    private float[][] lastPositionEmbeddings;
    private float dropoutRate;

    public int VocabSize { get => vocabSize; set => vocabSize = value; }
    public int BlockSize => blockSize;
    public int EmbedDim => embedDim;
    public int NumLayers => blocks.Count;
    public int NumHeads => blocks[0].GetNumHeads();

    public GPTModel(int vocabSize, int blockSize, int embedDim, int numLayers, int numHeads, AdamOptimizer optimizer, float clip, float dropoutRate = 0.1f)
    {
        this.vocabSize = vocabSize;
        this.blockSize = blockSize;
        this.embedDim = embedDim;
        this.dropoutRate = dropoutRate;

        tokenEmbedLinear = new Linear(vocabSize, embedDim, optimizer, clip);
        positionEmbedLinear = new Linear(blockSize, embedDim, optimizer, clip);

        tokenEmbeddings = new float[vocabSize][];
        tokenEmbeddingGrads = new float[vocabSize][];
        for (int i = 0; i < vocabSize; i++)
        {
            tokenEmbeddings[i] = new float[embedDim];
            tokenEmbeddingGrads[i] = new float[embedDim];
            for (int j = 0; j < embedDim; j++)
                tokenEmbeddings[i][j] = RandomUtils.RandNormal(0, 0.02f);
        }

        positionEmbeddings = new float[blockSize][];
        positionEmbeddingGrads = new float[blockSize][];
        for (int i = 0; i < blockSize; i++)
        {
            positionEmbeddings[i] = new float[embedDim];
            positionEmbeddingGrads[i] = new float[embedDim];
            for (int j = 0; j < embedDim; j++)
                positionEmbeddings[i][j] = RandomUtils.RandNormal(0, 0.02f);
        }

        transformerBlocks = new List<TransformerBlock>();
        for (int i = 0; i < numLayers; i++)
        {
            transformerBlocks.Add(new TransformerBlock(embedDim, numHeads, optimizer, clip, dropoutRate));
        }

        blocks = Enumerable.Range(0, numLayers)
            .Select(_ => new TransformerBlock(embedDim, numHeads, optimizer, clip, dropoutRate))
            .ToList();

        norm = new LayerNorm(embedDim);
        lmHead = new Linear(embedDim, vocabSize, optimizer, clip);
    }
}

public static class RandomUtils
{
    private static Random rng = new Random();

    public static float RandNormal(float mean = 0.0f, float stddev = 1.0f)
    {
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                               Math.Sin(2.0 * Math.PI * u2);
        return mean + stddev * (float)randStdNormal;
    }
}

public class LinearParams
{
    public float[][] weights { get; set; }
    public float[] biases { get; set; }
}

public static float[] SoftmaxWithTemperature(float[] logits, float temperature)
{
    float max = logits.Max();
    var exps = logits.Select(v => (float)Math.Exp((v - max) / temperature)).ToArray();
    float sum = exps.Sum();
    return exps.Select(v => v / sum).ToArray();
}

public class TransformerBlock
{
    public MultiHeadAttention mha;
    public LayerNorm norm1, norm2;
    public FeedForward ff;
    public float[][] xAfterMHA, xAfterFF;
    public float dropoutRate;

    public TransformerBlock(int dim, int heads, AdamOptimizer optimizer, float clip, float dropoutRate)
    {
        mha = new MultiHeadAttention(dim, heads, optimizer, clip);
        ff = new FeedForward(dim, optimizer, clip);
        norm1 = new LayerNorm(dim);
        norm2 = new LayerNorm(dim);
        this.dropoutRate = dropoutRate;
    }

    public float[][] Forward(float[][] x)
    {
        var attn = mha.Forward(x);
        xAfterMHA = MatrixUtils.Add(x, attn);
        xAfterMHA = Dropout.Apply(xAfterMHA, dropoutRate);
        var normed1 = norm1.Forward(xAfterMHA);

        var ffOut = ff.Forward(normed1);
        xAfterFF = MatrixUtils.Add(normed1, ffOut);
        xAfterFF = Dropout.Apply(xAfterFF, dropoutRate);
        return norm2.Forward(xAfterFF);
    }

    public float[][] Backward(float[][] gradOut)
    {
        var gradNorm2 = norm2.Backward(gradOut);
        var gradFF = ff.Backward(gradNorm2);
        var gradSum1 = MatrixUtils.Add(gradFF, gradNorm2);
        var gradNorm1 = norm1.Backward(gradSum1);
        var gradMHA = mha.Backward(gradNorm1);
        return MatrixUtils.Add(gradMHA, gradNorm1);
    }
}

public class MultiHeadAttention
{
    private int numHeads;
    private int headDim;
    private List<Linear> q, k, v;
    public Linear output;

    private float[][] input;
    private List<float[][]> Qs, Ks, Vs, Softmaxed;

    public MultiHeadAttention(int embedDim, int numHeads, AdamOptimizer optimizer, float clip)
    {
        this.numHeads = numHeads;
        this.headDim = embedDim / numHeads;

        q = new(); k = new(); v = new();
        for (int i = 0; i < numHeads; i++)
        {
            q.Add(new Linear(embedDim, headDim, optimizer, clip));
            k.Add(new Linear(embedDim, headDim, optimizer, clip));
            v.Add(new Linear(embedDim, headDim, optimizer, clip));
        }

        output = new Linear(embedDim, embedDim, optimizer, clip);
    }

    public float[][] Forward(float[][] x)
    {
        input = x;
        Qs = new(); Ks = new(); Vs = new(); Softmaxed = new();

        var heads = new List<float[][]>();

        for (int i = 0; i < numHeads; i++)
        {
            var Q = q[i].Forward(x);
            var K = k[i].Forward(x);
            var V = v[i].Forward(x);
            Qs.Add(Q); Ks.Add(K); Vs.Add(V);

            var headOutput = new float[x.Length][];
            var softList = new List<float[]>();

            for (int t = 0; t < Q.Length; t++)
            {
                var scores = new float[K.Length];
                for (int j = 0; j < K.Length; j++)
                    scores[j] = MatrixUtils.Dot(Q[t], K[j]);

                var soft = MatrixUtils.Softmax(scores);
                softList.Add(soft);

                var weighted = new float[V[0].Length];
                for (int j = 0; j < V.Length; j++)
                    for (int d = 0; d < V[0].Length; d++)
                        weighted[d] += soft[j] * V[j][d];

                headOutput[t] = weighted;
            }

            Softmaxed.Add(softList.ToArray());
            heads.Add(headOutput);
        }

        var combined = new float[x.Length][];
        for (int t = 0; t < x.Length; t++)
        {
            combined[t] = new float[numHeads * headDim];
            for (int h = 0; h < numHeads; h++)
                for (int d = 0; d < headDim; d++)
                    combined[t][h * headDim + d] = heads[h][t][d];
        }

        return output.Forward(combined);
    }
}

public class FeedForward
{
    public Linear fc1, fc2;
    private float[][] reluInput;

    public FeedForward(int dim, AdamOptimizer optimizer, float clip)
    {
        fc1 = new Linear(dim, dim * 4, optimizer, clip);
        fc2 = new Linear(dim * 4, dim, optimizer, clip);
    }

    public float[][] Forward(float[][] x)
    {
        reluInput = fc1.Forward(x);
        var act = reluInput.Select(MatrixUtils.ReLU).ToArray();
        return fc2.Forward(act);
    }

    public float[][] Backward(float[][] gradOut)
    {
        var gradAct = fc2.Backward(gradOut);
        var gradReLU = new float[gradAct.Length][];
        for (int i = 0; i < gradAct.Length; i++)
        {
            gradReLU[i] = new float[gradAct[i].Length];
            for (int j = 0; j < gradAct[i].Length; j++)
                gradReLU[i][j] = reluInput[i][j] > 0 ? gradAct[i][j] : 0;
        }
        return fc1.Backward(gradReLU);
    }
}

public static float[] ApplyRepetitionPenalty(float[] logits, HashSet<int> seenTokens, float penalty)
{
    var penalized = new float[logits.Length];
    for (int i = 0; i < logits.Length; i++)
    {
        if (seenTokens.Contains(i))
            penalized[i] = logits[i] / penalty;
        else
            penalized[i] = logits[i];
    }
    return penalized;
}

public static int SampleFromTopP(float[] probs, float p)
{
    var sorted = probs
        .Select((prob, idx) => new { Index = idx, Prob = prob })
        .OrderByDescending(x => x.Prob)
        .ToList();

    var topPList = new List<(int Index, float Prob)>();
    float cumulative = 0.0f;

    foreach (var item in sorted)
    {
        cumulative += item.Prob;
        topPList.Add((item.Index, item.Prob));
        if (cumulative >= p) break;
    }

    float total = topPList.Sum(x => x.Prob);
    float r = (float)new Random().NextDouble();
    float cum = 0;

    foreach (var item in topPList)
    {
        cum += item.Prob / total;
        if (r < cum) return item.Index;
    }

    return topPList.Last().Index;
}

public static int SampleFromTopK(float[] probs, int k)
{
    var indexed = probs
        .Select((p, idx) => (p, idx))
        .OrderByDescending(x => x.p)
        .Take(k)
        .ToList();

    var topKSum = indexed.Sum(x => x.p);
    var normalized = indexed
        .Select(x => (x.idx, prob: x.p / topKSum))
        .ToList();

    float r = (float)new Random().NextDouble();
    float cum = 0;
    foreach (var (idx, prob) in normalized)
    {
        cum += prob;
        if (r < cum) return idx;
    }

    return normalized.Last().idx;
}

public static int SampleTopKTopP(float[] probs, int topK, float topP, Random rand)
{
    var indexed = probs
        .Select((p, idx) => (p, idx))
        .OrderByDescending(x => x.p)
        .Take(topK)
        .ToList();

    float cumulative = 0;
    var topPFiltered = new List<(int idx, float prob)>();
    foreach (var (p, idx) in indexed)
    {
        cumulative += p;
        topPFiltered.Add((idx, p));
        if (cumulative >= topP)
            break;
    }

    float sum = topPFiltered.Sum(x => x.prob);
    var normalized = topPFiltered
        .Select(x => (x.idx, prob: x.prob / sum))
        .ToList();

    float r = (float)rand.NextDouble();
    float cumProb = 0;
    foreach (var (idx, prob) in normalized)
    {
        cumProb += prob;
        if (r < cumProb) return idx;
    }

    return normalized.Last().idx;
} 