using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Niazza.SentinelRedis.Sentinel
{
    internal class SentinelClient : ISentinelClient
    {
        private readonly RedisConfiguration _configuration;

        public SentinelClient(RedisConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<ConnectionMultiplexer> CreateConnectionAsync()
        {
            var configurationOptions = new ConfigurationOptions
            {
                AsyncTimeout = 10000,
                CommandMap =  CommandMap.Sentinel,
                ConnectRetry = 3,
                TieBreaker = string.Empty,
                EndPoints = { {_configuration.SentinelHost, _configuration.SentinelPort} }
            };

            var muxer = await ConnectionMultiplexer.ConnectAsync(configurationOptions).ConfigureAwait(false);

            return muxer;
        }

        public async Task<EndPoint> GetRedisMasterEndpointAsync()
        {
            var sentinelConnection = await CreateConnectionAsync();
            if (!sentinelConnection.IsConnected)
                throw new SentinelMuxerConnectionException(_configuration.SentinelHost, _configuration.SentinelPort);

            var server = sentinelConnection.GetServer(_configuration.SentinelHost, _configuration.SentinelPort);
            var redisEndpoint = await server.SentinelGetMasterAddressByNameAsync(_configuration.ServiceName, CommandFlags.DemandMaster).ConfigureAwait(false);
            return redisEndpoint;
        } 

    }
}
