using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;

namespace Niazza.SentinelRedis.Tests
{
    [TestClass]
    public class RedisCommandsExecutorTests
    {

        [TestMethod]
        public async Task CheckIfExecuteReturnCorrectValue()
        {
            var redisDbManagerMock = new Mock<IRedisDbManager>();
            var loggerMock = new Mock<ILogger<RedisCommandsExecutor>>();
            var commandExecutor = new RedisCommandsExecutor(redisDbManagerMock.Object, loggerMock.Object);
            var expected = 1;
            var result = await commandExecutor.ExecuteAsync(db => Task.FromResult(expected));
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public async Task ConnectionExceptionThrownAndFixed()
        {
            var redisDbManagerMock = new Mock<IRedisDbManager>();
            var dbConnectionMock = new Mock<IDatabaseAsync>();
            redisDbManagerMock.Setup(x => x.GetDatabaseAsync()).Returns(() => Task.FromResult(dbConnectionMock.Object));
            var loggerMock = new Mock<ILogger<RedisCommandsExecutor>>();
            var commandExecutor = new RedisCommandsExecutor(redisDbManagerMock.Object, loggerMock.Object);
            await commandExecutor.ExecuteAsync(db => Task.CompletedTask);
            var isEx = true;
            await commandExecutor.ExecuteAsync(db =>
            {
                if (isEx)
                {
                    isEx = false;
                    return Task.FromException(new RedisTimeoutException("", CommandStatus.Sent));
                }
                return Task.CompletedTask;
            });
            
            redisDbManagerMock.Verify(x => x.GetDatabaseAsync(), Times.Exactly(2));
        }


        [TestMethod]
        [ExpectedException(typeof(RedisTimeoutException))]
        public async Task RedisTimeoutExceptionThrown()
        {
            var redisDbManagerMock = new Mock<IRedisDbManager>();
            var dbConnectionmock = new Mock<IDatabaseAsync>();
            redisDbManagerMock.Setup(x => x.GetDatabaseAsync()).Returns(() => Task.FromResult(dbConnectionmock.Object));
            var loggerMock = new Mock<ILogger<RedisCommandsExecutor>>();
            var commandExecutor = new RedisCommandsExecutor(redisDbManagerMock.Object, loggerMock.Object);
            await commandExecutor.ExecuteAsync(db => Task.CompletedTask);
            await commandExecutor.ExecuteAsync(db =>Task.FromException(new RedisTimeoutException("", CommandStatus.Sent)));
            
            redisDbManagerMock.Verify(x => x.GetDatabaseAsync(), Times.Exactly(2));
        }



        [TestMethod]
        public async Task RunRedisExecutorTests()
        {

            var redisDbManagerMock = new Mock<IRedisDbManager>();
            var loggerMock = new Mock<ILogger<RedisCommandsExecutor>>();

            var commandExecutor = new RedisCommandsExecutor(redisDbManagerMock.Object, loggerMock.Object);
            await commandExecutor.ExecuteAsync(db => Task.CompletedTask);
            await commandExecutor.ExecuteAsync(db => Task.CompletedTask);
            redisDbManagerMock.Verify(x => x.GetDatabaseAsync(), Times.Exactly(1));

            

        }


    }
    
}
