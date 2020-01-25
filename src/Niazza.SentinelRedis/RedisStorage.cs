using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Niazza.SentinelRedis
{
    public class RedisStorage : IRedisStorage
    {
        private readonly IRedisCommandsExecutor _commandsExecutor;

        public RedisStorage(IRedisCommandsExecutor commandsExecutor)
        {
            _commandsExecutor = commandsExecutor;
        }


        public Task<TResult> GetAsync<TResult>(string key)
        {
            return _commandsExecutor.ExecuteAsync(async db =>
            {
                var result = await db.StringGetAsync(key);
                return result.HasValue ? JsonConvert.DeserializeObject<TResult>(result) : default(TResult);
            });
        }

        public Task AddAsync<TData>(string key, TData data, TimeSpan? expiry = null)
        {
            return _commandsExecutor.ExecuteAsync(async db =>
            {
                await db.StringSetAsync(key, JsonConvert.SerializeObject(data), expiry);
            });
        }

        public Task DeleteAsync(string key)
        {
            return _commandsExecutor.ExecuteAsync(db => db.KeyDeleteAsync(key));
        }
            
    }
}
