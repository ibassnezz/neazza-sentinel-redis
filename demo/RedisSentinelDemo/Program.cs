using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Niazza.SentinelRedis;

namespace RedisSentinelDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            // configuring Redis-Sentinel
            services.AddRedisSentinelCollection(new RedisConfiguration
            {
                DbNum = 0,
                Passkey = "qwertyuiop1234567890",
                ServiceName = "AnyServiceName",
                SentinelHost = "sentinel_host",
                SentinelPort = 26379
            });

            var serviceProvider = services.BuildServiceProvider();

            //service for Set Get Delete key-values
            var storage = serviceProvider.GetService<IRedisStorage>();

            var key = $"test_key{Guid.NewGuid():N}";
            var value = "Test";
            storage.AddAsync(key, value).GetAwaiter().GetResult();

            var data = storage.GetAsync<string>(key).GetAwaiter().GetResult();

            Assert.AreEqual(value, data);

            storage.DeleteAsync(key).GetAwaiter().GetResult();

            data = storage.GetAsync<string>(key).GetAwaiter().GetResult();

            Assert.IsNull(data);

            // if IRedisStorage does not correspond logic you may use action wrapper
            var commandsExecutor = serviceProvider.GetService<IRedisCommandsExecutor>();

            commandsExecutor.ExecuteAsync(db => db.StringSetAsync(key, value)).GetAwaiter().GetResult();

            var result = commandsExecutor.ExecuteAsync(db => db.StringGetSetAsync(key, value)).GetAwaiter().GetResult();
        }
    }
}
