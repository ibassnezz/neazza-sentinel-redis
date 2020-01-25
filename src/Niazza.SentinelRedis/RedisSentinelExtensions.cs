using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Niazza.SentinelRedis.Sentinel;

[assembly: InternalsVisibleTo("Niazza.SentinelRedis.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Niazza.SentinelRedis
{

    public static class RedisSentinelExtensions
    {
        public static IServiceCollection AddRedisSentinelCollection(this IServiceCollection services, RedisConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddSingleton<IRedisCommandsExecutor, RedisCommandsExecutor>();
            services.AddSingleton<ISentinelClient, SentinelClient>();
            services.AddSingleton<IRedisDbManager, RedisDbManager>();
            services.AddSingleton<IRedisStorage, RedisStorage>();
            services.AddLogging();
            return services;
        }
    }
}
