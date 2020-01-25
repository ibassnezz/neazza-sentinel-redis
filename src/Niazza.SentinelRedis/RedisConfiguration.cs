namespace Niazza.SentinelRedis
{
    public class RedisConfiguration
    {
        /// <summary>
        /// Default host
        /// </summary>
        public string SentinelHost { get; set; }

        /// <summary>
        /// Sentinel port 26379
        /// </summary>
        public int SentinelPort { get; set; } = 26379;

        /// <summary>
        /// ServiceName in Sentinel
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Access key for REDIS
        /// </summary>
        public string Passkey { get; set; }

        /// <summary>
        /// Db number default = 0
        /// </summary>
        public int DbNum { get; set; } = 0;

    }
}
