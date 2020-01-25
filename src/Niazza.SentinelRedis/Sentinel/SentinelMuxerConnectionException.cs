using System;

namespace Niazza.SentinelRedis.Sentinel
{
    public sealed class SentinelMuxerConnectionException: Exception 
    {
        public SentinelMuxerConnectionException(string host, int port): base("Sentinel connection is closed")
        {
            Data.Add("SentinelHost", $"{host}:{port}");
        }
    }
}
