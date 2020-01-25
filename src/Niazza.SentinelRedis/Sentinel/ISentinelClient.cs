using System.Net;
using System.Threading.Tasks;

namespace Niazza.SentinelRedis.Sentinel
{
    internal interface ISentinelClient
    {
        Task<EndPoint> GetRedisMasterEndpointAsync();
    }
}