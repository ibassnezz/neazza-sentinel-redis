using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Niazza.SentinelRedis.Sentinel;
using StackExchange.Redis;

namespace Niazza.SentinelRedis
{
    internal class RedisDbManager : IRedisDbManager
    {
        private readonly ISentinelClient _sentinelClient;
        private readonly RedisConfiguration _configuration;
        private readonly ILogger<RedisDbManager> _logger;

        public RedisDbManager(ISentinelClient sentinelClient, RedisConfiguration configuration, ILogger<RedisDbManager> logger)
        {
            _sentinelClient = sentinelClient;
            _configuration = configuration;
            _logger = logger;
        }

        private async Task<ConnectionMultiplexer> GetRedisConnectionAsync()
        {
            var endpoint = await _sentinelClient.GetRedisMasterEndpointAsync().ConfigureAwait(false);

            _logger.LogInformation($"Current Redis address is {endpoint}");

            var configurationOptions = new ConfigurationOptions
            {
                Password = _configuration.Passkey,
                AsyncTimeout = 5000,
                CommandMap = CommandMap.Default,
                ConnectRetry = 2,
                EndPoints = { endpoint }
            };

            var muxer = await ConnectionMultiplexer.ConnectAsync(configurationOptions).ConfigureAwait(false);

            if (!muxer.IsConnected)
                throw new RedisMuxerConnectionException(endpoint.ToString());
            
            return muxer;
        }

        public async Task<IDatabaseAsync> GetDatabaseAsync()
        {
            var connection = await GetRedisConnectionAsync();
            _logger.LogInformation("New Redis DB connection requested");

            return connection.GetDatabase(_configuration.DbNum);
        }
    }
}
