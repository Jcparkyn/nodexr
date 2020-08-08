using System.Collections.Generic;
using System.Linq;

namespace Nodexr.Shared
{
    public static class ExtensionMethods
    {
        public static Vector2 GetClientPos(this Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
        {
            return new Vector2(e.ClientX, e.ClientY);
        }

        public static string InNonCapturingGroup(this string input) => $"(?:{input})";

        public static bool IsSingleRegexChar(this string input)
        {
            return input.Length <= 1 || (input.Length == 2 && input.StartsWith("\\"));
        }

        public static string EscapeCharacters(this string input, IEnumerable<char> chars)
        {
            string result = "";
            for(int i = 0; i < input.Length; i++)
            {
                if (chars.Contains(input[i]))
                {
                    //Escape the current character
                    result += @"\";
                }
                result += input[i];
            }
            return result;
        }
    }
}
