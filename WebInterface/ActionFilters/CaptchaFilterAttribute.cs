using Microsoft.AspNetCore.Mvc.Filters;
using WebInterface.Utils;
using System.Reflection;

namespace WebInterface.ActionFilters
{
    /// <summary>
    /// Handles Google ReCaptcha check.
    /// Adds error to model state if captcha check fails
    /// </summary>
    public class CaptchaFilterAttribute : ActionFilterAttribute
    {
        public string ErrorMessage { get; set; } = "Captcha is not valid";


        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            CaptchaValidator captchaValidator = context.HttpContext.RequestServices.GetRequiredService<CaptchaValidator>();

            var captchaInputValue = context.HttpContext.Request.Form["g-recaptcha-response"];

            if (!await captchaValidator.ValidateCaptcha(captchaInputValue))
            {
                context.ModelState.AddModelError("", ErrorMessage);
            }

            await next();
        }
    }
}
