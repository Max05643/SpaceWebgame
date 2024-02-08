using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebInterface.Utils;

namespace WebInterface.TagHelpers
{
    [HtmlTargetElement("captcha", TagStructure = TagStructure.WithoutEndTag)]
    public class CaptchaTagHelper : TagHelper
    {
        private readonly CaptchaValidator captchaValidator;

        public CaptchaTagHelper(CaptchaValidator captchaValidator)
        {
            this.captchaValidator = captchaValidator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = null;
            if (captchaValidator.IsEnabled)
            {
                var scriptTag = new TagBuilder("script");
                scriptTag.Attributes.Add("src", @"https://www.google.com/recaptcha/api.js");
                scriptTag.Attributes.Add("defer", "true");

                var divTag = new TagBuilder("div");
                divTag.AddCssClass("g-recaptcha");
                divTag.Attributes.Add("data-sitekey", captchaValidator.SiteKey);


                output.PostContent.AppendHtml(scriptTag);
                output.PostContent.AppendHtml(divTag);
            }
        }
    }
}
