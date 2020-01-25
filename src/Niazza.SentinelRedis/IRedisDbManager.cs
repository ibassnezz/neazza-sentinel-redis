using System.Threading.Tasks;
using StackExchange.Redis;

namespace Niazza.SentinelRedis
{
    internal interface IRedisDbManager
    {
        Task<IDatabaseAsync> GetDatabaseAsync();
    }
}