using StackExchange.Redis;

namespace k8sFormApp.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public async Task AddPersonAsync(string personJson)
        {
            // Use a list to queue persons for processing
            //await _database.ListRightPushAsync("persons_queue", personJson);
            await _database.ListRightPushAsync("persons_queue", personJson);

            //await _database.StringSetAsync("Test", "Hello");
            //var value = await _database.StringGetAsync("Test");
        }
    }
}
