using System.Collections.Generic;
using System.Linq;
using BlazorNodes.Core;

namespace Nodexr.Utils
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

        public static string EscapeSpecialCharacters(this string input, bool escapeBackslashes = true)
        {
            const string specialCharacters = "()[]{}$^?.+*|";

            string charsToEscape = escapeBackslashes ?
                specialCharacters + "\\" :
                specialCharacters;

            return EscapeCharacters(input, charsToEscape);
        }
    }
}
