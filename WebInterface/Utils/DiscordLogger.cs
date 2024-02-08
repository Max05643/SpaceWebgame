using System.Net.Http;

namespace WebInterface.Utils
{

    /// <summary>
    /// Sends log messages to separate application that sends them in discord channels
    /// </summary>
    public class DiscordLogger : ILogger
    {

        private readonly string endpoint;
        public DiscordLogger(IConfiguration config)
        {
            if (config["DiscordLogger:Endpoint"] == null)
                throw new ArgumentNullException("Can't find DiscordLogger:endpoint configuration");

            endpoint = config["DiscordLogger:Endpoint"]!;
        }

        IDisposable? ILogger.BeginScope<TState>(TState state) => default;

        bool ILogger.IsEnabled(LogLevel logLevel) => true;

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var logMessage = formatter(state, exception);

            Task.Run(async () => await SendMessage(new DiscordLoggerMessage() { Message = $"{logLevel}:{logMessage}" }));
        }


        async Task SendMessage(DiscordLoggerMessage message)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    await client.PostAsJsonAsync(endpoint, message);
                }
                catch
                { 
                }
            }
        }

        /// <summary>
        /// Represents a message to log
        /// </summary>
        [System.Serializable]
        class DiscordLoggerMessage
        {
            public string Message { get; set; } = string.Empty;
        }






    }
}
