using Microsoft.Extensions.AI;
using Microsoft.Extensions.ObjectPool;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics.Tensors;
using System.Threading;
using System.Threading.Tasks;

namespace LLMService
{
    public class BpeLocalEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>, IDisposable
    {
        private readonly Tokenizer _bpeTokenizer;
        private readonly string _onnxModelPath;
        private readonly ObjectPool<InferenceSession> _sessionPool;

        public BpeLocalEmbeddingGenerator()
        {
            string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "EmbedModel");

            // 加载分词器（线程安全）
            var vocabPath = Path.Combine(modelPath, "vocab.json");
            var mergesPath = Path.Combine(modelPath, "merges.txt");
            _bpeTokenizer = BpeTokenizer.Create(
                File.OpenRead(vocabPath),
                File.OpenRead(mergesPath));

            _onnxModelPath = Path.Combine(modelPath, "model.onnx");

            // 初始化会话池（核心：线程安全、高性能）
            var policy = new InferenceSessionPooledPolicy(_onnxModelPath);
            _sessionPool = new DefaultObjectPool<InferenceSession>(
                policy,
                Environment.ProcessorCount * 2); // 最大缓存会话数
        }

        public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
            IEnumerable<string> values,
            EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var embeddings = new List<Embedding<float>>();

                // 从会话池获取会话（线程安全）
                var session = _sessionPool.Get();
                try
                {
                    foreach (var text in values)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var ids = _bpeTokenizer.EncodeToIds(text);
                        int seqLen = ids.Count;
                        if (seqLen > 8192)
                        {
                            seqLen = 8192;
                            ids = ids.Take(seqLen).ToList();
                        }

                        if (seqLen == 0)
                        {
                            embeddings.Add(new Embedding<float>(Array.Empty<float>()));
                            continue;
                        }

                        long[] inputIdsArray = ids.Select(i => (long)i).ToArray();
                        long[] attentionMaskArray = Enumerable.Repeat(1L, seqLen).ToArray();
                        long[] positionIdsArray = Enumerable.Range(0, seqLen).Select(i => (long)i).ToArray();

                        var inputIds = new DenseTensor<long>(inputIdsArray, new[] { 1, seqLen });
                        var attentionMask = new DenseTensor<long>(attentionMaskArray, new[] { 1, seqLen });
                        var positionIds = new DenseTensor<long>(positionIdsArray, new[] { 1, seqLen });

                        var inputs = new List<NamedOnnxValue>
                        {
                            NamedOnnxValue.CreateFromTensor("input_ids", inputIds),
                            NamedOnnxValue.CreateFromTensor("attention_mask", attentionMask),
                            NamedOnnxValue.CreateFromTensor("position_ids", positionIds)
                        };

                        using var outputs = session.Run(inputs);
                        var vector = outputs.First().AsTensor<float>();

                        float[][] batchEmbeddings = ApplyMeanPooling(
                            vector,
                            new[] { attentionMaskArray },
                            1,
                            seqLen);

                        L2Normalize(batchEmbeddings[0]);
                        embeddings.Add(new Embedding<float>(batchEmbeddings[0]));
                    }
                }
                finally
                {
                    // 使用完归还到池
                    _sessionPool.Return(session);
                }

                return new GeneratedEmbeddings<Embedding<float>>(embeddings);
            }, cancellationToken);
        }

        private float[][] ApplyMeanPooling(Microsoft.ML.OnnxRuntime.Tensors.Tensor<float> outputTensor, long[][] attentionMasks, int batchSize, int sequenceLength)
        {
            var dimensions = outputTensor.Dimensions.ToArray();
            var hiddenSize = dimensions[^1];
            var embeddings = new float[batchSize][];
            var denseTensor = (DenseTensor<float>)outputTensor;
            var tensorSpan = denseTensor.Buffer.Span;

            for (int batch = 0; batch < batchSize; batch++)
            {
                var embedding = new float[hiddenSize];
                int tokenCount = 0;
                var masks = attentionMasks[batch];

                for (int seq = 0; seq < sequenceLength; seq++)
                {
                    if (masks[seq] == 0) continue;
                    tokenCount++;
                    int offset = (batch * sequenceLength + seq) * hiddenSize;
                    TensorPrimitives.Add(embedding, tensorSpan.Slice(offset, hiddenSize), embedding);
                }

                if (tokenCount > 0)
                    TensorPrimitives.Divide(embedding, (float)tokenCount, embedding);

                embeddings[batch] = embedding;
            }

            return embeddings;
        }

        private void L2Normalize(float[] vector)
        {
            var norm = TensorPrimitives.Norm(vector);
            if (norm > 0)
                TensorPrimitives.Divide(vector, norm, vector);
        }

        public void Dispose()
        {
            // 会话池释放
            if (_sessionPool is IDisposable disposable)
                disposable.Dispose();
        }

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            if (serviceType == typeof(IEmbeddingGenerator<string, Embedding<float>>))
                return this;
            return null;
        }
    }
    public class InferenceSessionPooledPolicy : IPooledObjectPolicy<InferenceSession>
    {
        private readonly string _modelPath;
        public InferenceSessionPooledPolicy(string modelPath) => _modelPath = modelPath;
        public InferenceSession Create() => new InferenceSession(_modelPath);
        public bool Return(InferenceSession obj) => obj != null;
    }

}
