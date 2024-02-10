using GameDesign.GameMaps;
using GameDesign.GameState;
using GameDesign.Models;
using GameServerImplementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebInterface.Utils
{
    /// <summary>
    /// Provides an abstraction over game server settings
    /// </summary>
    public static class GameServerSettingsFactory
    {

        public static GameServerSettings GetServerSettings(IConfiguration configuration)
        {
            var tickTime = int.Parse(configuration["Game:TargetTickTime"] ?? "33");
            var playerKickTimeout = int.Parse(configuration["Game:PlayerKickTimeoutInSeconds"] ?? "60");

            var result = new GameServerSettings() { TargetTickTimeMs = tickTime};


            return result;
        }
    }
}
