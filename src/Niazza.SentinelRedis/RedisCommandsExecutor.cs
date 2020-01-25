using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Niazza.SentinelRedis.Sentinel;
using Polly;
using Polly.Retry;
using StackExchange.Redis;

namespace Niazza.SentinelRedis
{
    internal class RedisCommandsExecutor : IRedisCommandsExecutor
    {
        private readonly IRedisDbManager _redisDbManager;
        private readonly ILogger<RedisCommandsExecutor> _logger;

        private volatile bool _isDbConnected = false;

        private readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        private IDatabaseAsync _databaseAsync = null;

        private readonly RetryPolicy _asyncRetryPolicy;

        public RedisCommandsExecutor(IRedisDbManager redisDbManager, ILogger<RedisCommandsExecutor> logger)
        {
            _redisDbManager = redisDbManager;
            _logger = logger;
            var policy = Policy.Handle<SentinelMuxerConnectionException>()
                .Or<RedisMuxerConnectionException>()
                .Or<RedisTimeoutException>()
                .Or<RedisConnectionException>()
                .Or<RedisCommandException>();

            _asyncRetryPolicy = policy.RetryAsync(3, async (exception, attempts) =>
            {
                _logger.LogError(exception, "Redis DB connection failed");
                _isDbConnected = false;
                await ReInitializeDb();
            });
        }

        private async Task ReInitializeDb()
        {
            await SemaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_isDbConnected) return;
                var db = await _redisDbManager.GetDatabaseAsync().ConfigureAwait(false);
                _databaseAsync = db;
                _isDbConnected = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot connect to Redis DB");
                throw;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<IDatabaseAsync, Task<TResult>> func)
        {
            return _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                if (!_isDbConnected)
                    await ReInitializeDb().ConfigureAwait(false);
                return await func(_databaseAsync);
            });
        }

        public Task ExecuteAsync(Func<IDatabaseAsync, Task> func)
        {
            return _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                if (!_isDbConnected)
                    await ReInitializeDb().ConfigureAwait(false);
                await func(_databaseAsync);
            });
        }
    }
}