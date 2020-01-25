using System;

namespace Niazza.SentinelRedis
{
    public sealed class RedisMuxerConnectionException: Exception
    {
        public RedisMuxerConnectionException(string endpoint): base("Connection loss in redis")
        {
            Data.Add("RedisHost", endpoint);
        }
    }
}
