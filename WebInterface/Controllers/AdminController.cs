using GameDesign.GameMaps;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xna.Framework;
using WebInterface.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebInterface.Controllers
{
    /// <summary>
    /// Handles admin panel
    /// </summary>
    public class AdminController : Controller
    {
        readonly ILogger<AdminController> logger;
        readonly FrontBackCommunication frontBackCommunication;


        public AdminController(ILogger<AdminController> logger, FrontBackCommunication frontBackCommunication)
        {
            this.logger = logger;
            this.frontBackCommunication = frontBackCommunication;
        }

        public IActionResult Panel()
        {
            return View();
        }


        [HttpGet]
        [Produces("application/json")]
        public ICollection<Guid> GetRunningServers()
        {
            return frontBackCommunication.GetCurrentGames();
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<int?> GetPlayersCount(Guid serverId)
        {
            return await frontBackCommunication.GetCurrentPlayersCount(serverId);
        }


        [HttpPost]
        public IActionResult StartNewGame()
        {
            frontBackCommunication.StartTheGame(new GameServersManager.Models.GameServerSettings(new GameDesign.Models.GameSettings()
            {
                Gravity = Vector2.Zero,
                TickTime = 16,
                WorldSize = Vector2.One * 800,
                ShouldDebugTickTime = false
            }));

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> StopGame([Required] Guid? gameId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await frontBackCommunication.StopTheGame(gameId!.Value);
            return Ok();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyMapToGame([FromForm] Guid? gameId, [FromForm] IFormFile? mapFile)
        {
            if (gameId == null || mapFile == null || mapFile.Length == 0)
            {
                return BadRequest();
            }

            using var readStream = mapFile.OpenReadStream();


            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new Vector2Converter());

            var map = JsonSerializer.Deserialize<GameMap>(readStream, options);
            
            if (map == null)
            {
                return BadRequest();
            }

            var result = await frontBackCommunication.ApplyMap(gameId.Value, map);

            if (result)
                return View("Panel");
            else
                return BadRequest();
        }

    }
}
