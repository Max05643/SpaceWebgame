using GameDesign.Utils;
using Microsoft.AspNetCore.Mvc;
using WebInterface.Utils;

namespace WebInterface.Controllers
{
    public class GameController : Controller
    {
        readonly ILogger<GameController> logger;
        readonly FrontBackCommunication gameServersManager;
        readonly IGraphicLibraryProvider graphicLibrary;
        
        public GameController(ILogger<GameController> logger, FrontBackCommunication gameServersManager, IGraphicLibraryProvider graphicLibrary)
        {
            this.logger = logger;
            this.gameServersManager = gameServersManager;
            this.graphicLibrary = graphicLibrary;
        }


        [HttpGet]
        public IActionResult GetGraphicLibrary()
        {
            return Json(graphicLibrary.GetLibrary());
        }

        [HttpGet]
        public IActionResult Kicked()
        {
            return View();
        }
        public async Task<IActionResult> Game()
        {
            var playerId = HttpContext.GetPlayerId();

            if (!playerId.HasValue)
            {
                return BadRequest();
            }

            if (!(await gameServersManager.IsPlayerInGame(playerId.Value)))
            {
                return BadRequest();
            }

            ViewData["playerId"] = playerId;
            return View();
        }
    }
}
