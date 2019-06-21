using System.Collections.Generic;
using System.Linq;

namespace RegexNodes.Shared
{
    public static class ExtensionMethods
    {
        public static Vector2L GetClientPos(this Microsoft.AspNetCore.Components.UIDragEventArgs e)
        {
            return new Vector2L(e.ClientX, e.ClientY);
        }

        public static string RemoveNonCapturingGroup(this string input)
        {
            //TODO: check for escaped end bracket
            if (input.StartsWith("(?:") && input.EndsWith(")"))
            {
                return input.Substring(3, input.Length - 4);
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Adds a non-capturing group if the input string is not wrapped in parentheses.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EnforceGrouped(this string input)
        {
            //TODO: check for escaped end bracket
            if (input.StartsWith("(") && input.EndsWith(")"))
            {
                return input;
            }
            else if (input.StartsWith("[") && input.EndsWith("]"))
            {
                return input;
            }
            else
            {
                return $"(?:{input})";
            }
        }

        public static string EscapeCharacters(this string input, IEnumerable<char> chars)
        {
            string result = "";
            for(int i = 0; i < input.Length; i++)
            {
                char curChar = input[i];
                if (chars.Contains(curChar))
                {
                    result += @"\" + curChar;
                }
                else
                {
                    result += curChar;
                }
            }
            return result;
        }
    }
}
