using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using GameDesign.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xna.Framework;
using WebInterface.ActionFilters;
using WebInterface.Models;
using WebInterface.Utils;

namespace WebInterface.Controllers;

public class HomeController : Controller
{
    readonly ILogger<HomeController> logger;
    readonly FrontBackCommunication gameServersManager;
    readonly CaptchaValidator captchaValidator;
    readonly LoadBalancer loadBalancer;

    public HomeController(ILogger<HomeController> logger, LoadBalancer loadBalancer, FrontBackCommunication gameServersManager, CaptchaValidator captchaValidator)
    {
        this.captchaValidator = captchaValidator;
        this.logger = logger;
        this.gameServersManager = gameServersManager;
        this.loadBalancer = loadBalancer;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult NickChange()
    {
        return View("NickChange", HttpContext.GetPlayerNick());
    }
    [HttpPost]
    public async Task<IActionResult> NickChange([FromForm, Required] string? nick)
    {

        if (nick != null && !UserTextInputValidator.ValidateNick(nick, out string? nickErrorMessage))
        {
            ModelState.AddModelError("nick", nickErrorMessage!);
        }

        if (!ModelState.IsValid)
            return View("NickChange", HttpContext.GetPlayerNick());

        await HttpContext.SetPlayerNick(nick!);

        return View("NickChange", nick);
    }

    [HttpGet]
    public IActionResult Settings()
    {
        return View();
    }





    [HttpGet]
    public IActionResult JoinGame()
    {
        return View();
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    [CaptchaFilter(ErrorMessage = "Captcha check failed")]
    [ActionName("JoinGame")]
    public async Task<IActionResult> JoinGamePost()
    {

        var playerId = HttpContext.GetPlayerId();
        var nick = HttpContext.GetPlayerNick();

        if (!playerId.HasValue || nick == null)
        {
            ModelState.AddModelError("", "Can't process cookies. Maybe, they are disabled?");
        }
        if (!ModelState.IsValid)
        {
            return View();
        }
        else
        {
            var serverId = await loadBalancer.GetServerForNewPlayer();
            await gameServersManager.JoinGame(serverId, playerId!.Value, nick!);
            return RedirectToAction("Game", "Game", new { serverId = serverId });
        }

    }

}
