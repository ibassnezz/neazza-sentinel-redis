# sentinel-redis-client

Redis under Sentinel management

The main reason of usage is to support stable redis connection to DB. For further information please read [Redis Sentinel Documentation](https://redis.io/topics/sentinel)

The package uses [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) as the base redis package to implement logic of safe usage of Redis

## Connection of Redis-Sentinel
```
 services.AddRedisSentinelCollection(new RedisConfiguration
            {
                DbNum = 0,
                Passkey = "qwertyuiop1234567890",
                ServiceName = "AnyServiceName",
                SentinelHost = "sentinel_host",
                SentinelPort = 26379
            });
```

## Set Get Delete
```
var storage = serviceProvider.GetService<IRedisStorage>();

var key = $"test_key{Guid.NewGuid():N}";
var value = "Test";
await storage.AddAsync(key, value);

var data = await storage.GetAsync<string>(key);

```

## Use wrapper to extended functionality
```
var commandsExecutor = serviceProvider.GetService<IRedisCommandsExecutor>();

await commandsExecutor.ExecuteAsync(db => db.StringSetAsync(key, value));

var result = await commandsExecutor.ExecuteAsync(db => db.StringGetSetAsync(key, value));
```
