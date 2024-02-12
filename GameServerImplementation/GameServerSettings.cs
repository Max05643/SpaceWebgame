using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation
{
    /// <summary>
    /// Provices configuration for game server
    /// </summary>
    public class GameServerSettings
    {
        /// <summary>
        /// Target time for one game tick. Server will try to observe it, but it is not guaranteed
        /// </summary>
        public int TargetTickTimeMs { get; set; } = 33;

        /// <summary>
        /// Player will be kicked if no input is received within specified time span
        /// </summary>
        public TimeSpan PlayerKickTimeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
