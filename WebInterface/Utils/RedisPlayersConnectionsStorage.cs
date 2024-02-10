using StackExchange.Redis;

namespace WebInterface.Utils
{
    /// <summary>
    /// Stores players' connections in redis
    /// </summary>
    public class RedisPlayersConnectionsStorage : IPlayersConnectionsStorage
    {
        readonly ConnectionMultiplexer connectionMultiplexer;
        readonly ILogger<RedisPlayersConnectionsStorage> logger;
        readonly Dictionary<string, LuaScript> scriptsPrepared = new Dictionary<string, LuaScript>();

        public RedisPlayersConnectionsStorage(IConfiguration config, ILogger<RedisPlayersConnectionsStorage> logger)
        {
            connectionMultiplexer = ConnectionMultiplexer.Connect(config.GetConnectionString("redis_connections")!);
            this.logger = logger;

            try
            {
                scriptsPrepared.Add("GetAllConnectionsByGame", LuaScriptsLoader.Load("GetAllConnectionsByGame"));
                scriptsPrepared.Add("GetAllConnectionsByGameAndPlayer", LuaScriptsLoader.Load("GetAllConnectionsByGameAndPlayer"));
                scriptsPrepared.Add("NewConnection", LuaScriptsLoader.Load("NewConnection"));
                scriptsPrepared.Add("RemovePlayer", LuaScriptsLoader.Load("RemovePlayer"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        async Task<string?> IPlayersConnectionsStorage.SwitchConnection(string playerKey, string connection)
        {
            try
            {
                var redisClient = connectionMultiplexer.GetDatabase();
                var args = new { gameKey = (RedisKey)"spacewar", playerKey = (RedisKey)playerKey, connection };
                var result = await redisClient.ScriptEvaluateAsync(scriptsPrepared["NewConnection"], args);

                if (result == null || result.IsNull)
                {
                    return null;
                }
                else
                {
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }

        async Task<string?> IPlayersConnectionsStorage.GetPlayersConnection(string playerKey)
        {
            try
            {
                var redisClient = connectionMultiplexer.GetDatabase();
                var args = new { gameKey = (RedisKey)"spacewar", playerKey = (RedisKey)playerKey };
                var result = await redisClient.ScriptEvaluateAsync(scriptsPrepared["GetAllConnectionsByGameAndPlayer"], args);

                if (result == null || result.IsNull)
                {
                    return null;
                }
                else
                {
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }

        async Task<List<string>> IPlayersConnectionsStorage.GetAllConnections()
        {
            try
            {
                var redisClient = connectionMultiplexer.GetDatabase();
                var args = new { gameKey = (RedisKey)"spacewar" };
                var result = await redisClient.ScriptEvaluateAsync(scriptsPrepared["GetAllConnectionsByGame"], args);
                var connections = ((RedisResult[])result)!.Select(r => r.ToString()).ToList();

                return connections!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new List<string>();
            }
        }

        async Task IPlayersConnectionsStorage.RemovePlayer(string playerKeyToRemove)
        {
            try
            {
                var redisClient = connectionMultiplexer.GetDatabase();
                var args = new { gameKey = (RedisKey)"spacewar", playerKey = (RedisKey)playerKeyToRemove };
                var result = await redisClient.ScriptEvaluateAsync(scriptsPrepared["RemovePlayer"], args);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }
    }
}
