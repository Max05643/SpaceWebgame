using System.Text.RegularExpressions;

namespace WebInterface.Utils
{
    /// <summary>
    /// Provides methods for validating textual information from clients 
    /// </summary>
    public static class UserTextInputValidator
    {

        static readonly Regex alphaNumericalOnly = new Regex(@"^[a-zA-Z0-9]+$", RegexOptions.Compiled);
        static readonly Regex alphaNumericalPunctiationOnly = new Regex(@"^[a-zA-Z0-9,.\:\-?!]+$", RegexOptions.Compiled);
        public static bool ValidateChatMessage(string message)
        {
            return message.Length <= 100 &&  message.Length > 0 && alphaNumericalPunctiationOnly.IsMatch(message);
        }

        public static bool ValidateNick(string nick, out string? errorMessage)
        {
            bool result = true;
            errorMessage = null;


            if (nick.Length >= 15)
            {
                result = false;
                errorMessage = "Nick is too long";
            }
            else if (nick.Length <= 2)
            {
                result = false;
                errorMessage = "Nick is too short";
            }
            else if (!alphaNumericalOnly.IsMatch(nick))
            {
                result = false;
                errorMessage = "Nick contains illegal characters";
            }


            return result;

        }

    }
}
