using System.Text.RegularExpressions;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace WebApplication1.Services.AI.VectorStore
{
    public class RedisVectorStore : IVectorStore
    {
        private readonly IDatabase _db;

        private const string INDEX_NAME = "product_vector_index";
        private const string PREFIX = "vector:";

        private const int VECTOR_DIMENSION = 384;

        public RedisVectorStore(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();

            CreateIndexIfNotExists();
        }

        private void CreateIndexIfNotExists()
        {
            try
            {
                var ft = _db.FT();

                ft.Info(INDEX_NAME);
            }
            catch
            {
                var schema = new Schema()
                    .AddTextField("content")
                    .AddVectorField(
                        "embedding",
                        Schema.VectorField.VectorAlgo.FLAT,
                        new Dictionary<string, object>
                        {
                            ["TYPE"] = "FLOAT32",
                            ["DIM"] = VECTOR_DIMENSION,
                            ["DISTANCE_METRIC"] = "COSINE"
                        });

                var options = new FTCreateParams()
                    .On(IndexDataType.HASH)
                    .Prefix(PREFIX);

                _db.FT().Create(INDEX_NAME, options, schema);
            }
        }

        private byte[] FloatArrayToByteArray(float[] vector)
        {
            var bytes = new byte[vector.Length * sizeof(float)];
            Buffer.BlockCopy(vector, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public async Task StoreAsync(
            string id,
            float[] embedding,
            string content,
            Dictionary<string, string>? metadata = null)
        {
            var key = PREFIX + id;

            var hash = new HashEntry[]
            {
            new("content", content),
            new("embedding", FloatArrayToByteArray(embedding))
            };

            await _db.HashSetAsync(key, hash);

            if (metadata != null)
            {
                foreach (var m in metadata)
                {
                    await _db.HashSetAsync(key, m.Key, m.Value);
                }
            }
        }

        public async Task<List<VectorSearchResult>> SearchAsync(
            float[] embedding,
            int topK = 5,
            string queryText = "")
        {
            // Construire un filtre safe pour RediSearch
            string filter;
            if (string.IsNullOrWhiteSpace(queryText))
            {
                filter = "*";
            }
            else
            {
                var tokens = Regex.Split(queryText.ToLowerInvariant(), @"\W+")
                                  .Where(t => t.Length > 1)
                                  .Take(6)
                                  .Distinct()
                                  .ToArray();

                filter = tokens.Length == 0
                    ? "*"
                    : string.Join(" | ", tokens.Select(t => $"@content:{t}*"));
            }

            var query = new Query($"*=>[KNN {topK} @embedding $vec AS score]")
                .AddParam("vec", FloatArrayToByteArray(embedding))
                .ReturnFields("content", "score")
                .Dialect(2);

            var result = await _db.FT().SearchAsync(INDEX_NAME, query);

            var list = new List<VectorSearchResult>();

            foreach (var doc in result.Documents)
            {
                // récupération sécurisée du score
                double score = 0;
                if (double.TryParse(doc["score"].ToString(), out var s))
                {
                    score = s;
                }

                // récupération sécurisée du content
                string content = doc["content"].ToString();

                list.Add(new VectorSearchResult
                {
                    Id = doc.Id.Replace(PREFIX, ""),
                    Content = content,
                    Score = score
                });
            }

            return list;
        }

        public async Task DeleteAsync(string id)
        {
            await _db.KeyDeleteAsync(PREFIX + id);
        }

        public async Task DeleteManyAsync(List<string> keys)
        {
            var redisKeys = keys.Select(k => (RedisKey)k).ToArray();

            await _db.KeyDeleteAsync(redisKeys);

        }

        public async Task<bool> ExistsKeyAsync(string id)
        {
            var key = PREFIX + id ;

            var exists = await _db.KeyExistsAsync(key);

            if (!exists)
                return false;

            var type = await _db.KeyTypeAsync(key);

            return type == RedisType.Hash;
        }

        public async Task<List<string>> GetChunkKeysByProductIdAsync(int productId)
        {
            var pattern = PREFIX + $"{productId}_chunk_*";
            var server = GetServer();

            var keys = server.Keys(pattern: pattern)
                              .Select(k => k.ToString())
                              .ToList() ;

             return keys;
        }

        public async Task<bool> IsEmptyAsync()
        {
            var server = GetServer();
            var keys = server.Keys(pattern: PREFIX + "*");

            return !keys.Any();
        }

        private IServer GetServer()
        {
            var multiplexer = (ConnectionMultiplexer)_db.Multiplexer;
            var endpoint = multiplexer.GetEndPoints().First();
            return multiplexer.GetServer(endpoint);
        }
    }
}
