namespace WebInterface.Utils
{
    public class DiscordLoggerProvider : ILoggerProvider
    {

        private readonly IConfiguration config;

        public DiscordLoggerProvider(IConfiguration config)
        {
            this.config = config;
        }

        ILogger ILoggerProvider.CreateLogger(string categoryName)
        {
            return new DiscordLogger(config);
        }

        void IDisposable.Dispose()
        {
        }
    }
}
