using WebInterface.Models;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebInterface.Utils
{
    /// <summary>
    /// Chat storage based on redis
    /// </summary>
    public class RedisChatStorage : IChatStorage
    {

        readonly int maxMessagesPerChatStored;
        readonly ConnectionMultiplexer connectionMultiplexer;
        readonly ILogger<RedisChatStorage> logger;
        readonly Dictionary<string, LuaScript> scriptsPrepared = new Dictionary<string, LuaScript>();

        public RedisChatStorage(IConfiguration config, ILogger<RedisChatStorage> logger)
        {

            if (config["Game:MaxStoredChatMessagesPerGame"] == null)
            {

                maxMessagesPerChatStored = 10;
                logger.LogWarning("Game:MaxStoredChatMessagesPerGame is not set, defaults to 10");
            }
            else
            {
                maxMessagesPerChatStored = int.Parse(config["Game:MaxStoredChatMessagesPerGame"]!);
            }


            connectionMultiplexer = ConnectionMultiplexer.Connect(config.GetConnectionString("redis_chat")!);
            this.logger = logger;

            try
            {
                scriptsPrepared.Add("AddNewChatMessage", LuaScriptsLoader.Load("AddNewChatMessage"));
                scriptsPrepared.Add("GetChatMessages", LuaScriptsLoader.Load("GetChatMessages"));
                scriptsPrepared.Add("RemoveChatMessagesFromServer", LuaScriptsLoader.Load("RemoveChatMessagesFromServer"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }


        public async Task AddNewChatMessage(Guid serverId, ChatMessage chatMessage)
        {
            try
            {
                var redisClient = connectionMultiplexer.GetDatabase();
                var args = new { gameKey = (RedisKey)serverId.ToString(), maxMessagesPerChatStored, message = JsonSerializer.Serialize(chatMessage) };
                await redisClient.ScriptEvaluateAsync(scriptsPrepared["AddNewChatMessage"], args);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        public async Task<ICollection<ChatMessageConainer>> GetChatMessages(Guid serverId, long id)
        {
            try
            {
                var redisClient = connectionMultiplexer.GetDatabase();
                var args = new { gameKey = (RedisKey)serverId.ToString(), id = id };
                var result = await redisClient.ScriptEvaluateAsync(scriptsPrepared["GetChatMessages"], args);



                if (result == null || result.IsNull)
                {
                    return Array.Empty<ChatMessageConainer>();
                }
                else
                {

                    List<ChatMessageConainer> resultList = new List<ChatMessageConainer>();
                    var inputArray = (RedisResult[])result!;

                    for (int i = 0; i < inputArray.Length; i += 2)
                    {
                        string currentId = ((string?)inputArray[i + 1])!;
                        resultList.Add(new ChatMessageConainer() { Id = long.Parse(currentId), Message = JsonSerializer.Deserialize<ChatMessage>(((string?)inputArray[i])!) });
                    }

                    return resultList;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return Array.Empty<ChatMessageConainer>();
            }
        }

        public async Task ShutDownServer(Guid serverId)
        {
            try
            {
                var redisClient = connectionMultiplexer.GetDatabase();
                var args = new { gameKey = (RedisKey)serverId.ToString() };
                await redisClient.ScriptEvaluateAsync(scriptsPrepared["RemoveChatMessagesFromServer"], args);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }
    }
}
