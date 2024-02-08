namespace WebInterface.Utils
{
    /// <summary>
    /// Adds random player_id key to session if the session is avaliable and the key is not present.
    /// Stores player's nickname in the session
    /// </summary>
    public class PlayerSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public PlayerSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Session.IsAvailable)
            {
                if (!httpContext.Session.Keys.Contains("player_id"))
                {
                    httpContext.Session.SetString("player_id", System.Guid.NewGuid().ToString());
                }
                if (!httpContext.Session.Keys.Contains("player_nick"))
                {
                    httpContext.Session.SetString("player_nick", "Player");
                }
                await httpContext.Session.CommitAsync();
            }
            await _next(httpContext);
        }
    }
    public static class PlayerSessionExtensions
    {
        /// <summary>
        /// Adds PlayerIdMiddleware
        /// </summary>
        public static IApplicationBuilder UsePlayerSessionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PlayerSessionMiddleware>();
        }

        /// <summary>
        /// Gets player's nick from session or returns null if it is not present or can not be obtained
        /// </summary>
        public static string? GetPlayerNick(this HttpContext context)
        {
            if (context.Session.IsAvailable && context.Session.Keys.Contains("player_nick"))
            {
                return context.Session.GetString("player_nick");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets player's nick into session
        /// </summary>
        /// <returns>Whether operation was completed</returns>
        public static async Task<bool> SetPlayerNick(this HttpContext context, string nick)
        {
            if (context.Session.IsAvailable)
            {
                context.Session.SetString("player_nick", nick);
                await context.Session.CommitAsync();
                return true;
            }
            else
            {
                return false;
            }
        }




        /// <summary>
        /// Gets player's id from session or returns null if it is not present or can not be obtained
        /// </summary>
        public static System.Guid? GetPlayerId(this HttpContext context)
        {
            if (context.Session.IsAvailable)
            {
                if (System.Guid.TryParse(context.Session.GetString("player_id"), out Guid result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
