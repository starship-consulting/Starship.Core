using System.Linq;
using System.Text.RegularExpressions;

namespace Starship.Core.Validation {
    public static class Validation {
        
        public static bool IsAlphaNumeric(string text) {
            return Regex.IsMatch(text, "^[a-zA-Z][a-zA-Z0-9]*$") && !ContainsEscapeCharacters(text);
        }

        public static bool ContainsEscapeCharacters(string text) {
            return text.Any(char.IsControl);
        }

        public static bool Email(string email) {

            if (ContainsEscapeCharacters(email)) {
                return false;
            }

            var match = Regex.IsMatch(email, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

            return match;
        }
    }
}
