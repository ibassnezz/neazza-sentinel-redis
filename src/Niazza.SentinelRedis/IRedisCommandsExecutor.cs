using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Niazza.SentinelRedis
{
    public interface IRedisCommandsExecutor
    {
        Task<TResult> ExecuteAsync<TResult>(Func<IDatabaseAsync, Task<TResult>> func);
        Task ExecuteAsync(Func<IDatabaseAsync, Task> func);
    }
}