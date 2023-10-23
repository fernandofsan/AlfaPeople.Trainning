using System.Text.RegularExpressions;

namespace AlfaPeople.Trainning.Plugins
{
    public static class StringExtensions
    {
        public static string ToOnlyNumbers(this string value)
        {
            string response = string.Empty;

            var matches = Regex.Matches(value, @"\d+");

            foreach (var match in matches)
                response += match;

            return response;

        }
    }
}
