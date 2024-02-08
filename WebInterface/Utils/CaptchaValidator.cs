namespace WebInterface.Utils
{
    /// <summary>
    /// Validates captcha from GooglereCAPTCHA
    /// </summary>
    public class CaptchaValidator
    {


        /// <summary>
        /// Used for data from Google's servers
        /// </summary>
        class ValidationResult
        {
            public bool? Success { get; set; } = null;
            public string? Hostname { get; set; } = null;
        }


        /// <summary>
        /// Can captcha be used?
        /// </summary>
        public bool IsEnabled => isEnabled;


        /// <summary>
        /// Current site key used in reCaptcha
        /// </summary>
        public string SiteKey => siteKey;

        const string apiUrl = "https://www.google.com/recaptcha/api/siteverify";
        readonly string siteKey;
        readonly string secret;
        readonly ILogger<CaptchaValidator> logger;
        readonly bool isEnabled;

        public CaptchaValidator(IConfiguration configuration, ILogger<CaptchaValidator> logger)
        {
            this.logger = logger;

            if (configuration["Captcha:Enabled"] == null || configuration["Captcha:Enabled"] != "true")
            {
                isEnabled = false;
                siteKey = "";
                secret = "";
            }
            else if (configuration["Captcha:CaptchaSiteKey"] == null || configuration["Captcha:CaptchaSecret"] == null)
            {
                logger.LogError("Can't read configuration while initializing CaptchaValidator");
                isEnabled = false;
                siteKey = "";
                secret = "";
            }
            else
            {
                isEnabled = true;
                siteKey = configuration["Captcha:CaptchaSiteKey"]!;
                secret = configuration["Captcha:CaptchaSecret"]!;
            }

        }


        /// <summary>
        /// Validates captcha
        /// </summary>
        /// <param name="userResponse">g-recaptcha-response post parameter from user's request</param>
        public async Task<bool> ValidateCaptcha(string? userResponse)
        {

            if (!isEnabled)
                return true;

            if (userResponse == null)
            {
                return false;
            }

            using (HttpClient client = new HttpClient())
            {
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", secret),
                    new KeyValuePair<string, string>("response", userResponse),
                });

                HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

                if (response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadFromJsonAsync<ValidationResult>();

                    if (result == null)
                        return false;
                    if (!result.Success.HasValue)
                        return false;

                    return result.Success.Value;
                }
                else
                {
                    logger.LogError("Error while validating captcha: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
        }
    }
}
