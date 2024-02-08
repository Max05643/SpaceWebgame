using StackExchange.Redis;
using System.Reflection;
using System.Text;
using System.Text.Unicode;

namespace WebInterface.Utils
{
    /// <summary>
    /// Helps to load scripts to redis
    /// </summary>
    public static class LuaScriptsLoader
    {

        /// <summary>
        /// Prepares script from LuaScripts for using with redisServer and returns obtained handler object
        /// </summary>
        public static LuaScript Load(string scriptName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream($"WebInterface.LuaScripts.{scriptName}"))
            {
                if (stream == null)
                    throw new InvalidOperationException("Resource not found");

                using (var reader = new StreamReader(stream))
                {
                    return LuaScript.Prepare(reader.ReadToEnd());
                }
            }
        }
    }
}
