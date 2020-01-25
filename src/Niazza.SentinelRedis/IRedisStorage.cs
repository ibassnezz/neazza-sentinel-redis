using System;
using System.Threading.Tasks;

namespace Niazza.SentinelRedis
{
    public interface IRedisStorage
    {
        Task<TResult> GetAsync<TResult>(string key);
        Task AddAsync<TData>(string key, TData data, TimeSpan? expiry = null);
        Task DeleteAsync(string key);
    }
}