using System.Collections.Generic;
using System.Linq;

namespace RegexNodes.Shared
{
    public static class ExtensionMethods
    {
        public static Vector2L GetClientPos(this Microsoft.AspNetCore.Components.Web.DragEventArgs e)
        {
            return new Vector2L((long)e.ClientX, (long)e.ClientY);
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
