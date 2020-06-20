using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RegexNodes.Shared.NodeTypes
{
    public class TextNode : Node
    {
        public override string Title => "Text";
        public override string NodeInfo => "Inserts text into your Regex which will be interpreted literally, " +
            "so all special characters are escaped with a backslash. E.g. $25.99? becomes \\$25\\.99\\?" +
            "\nNote: Backslash characters (\\), and the character immediately following them, are not escaped." +
            "\nTo insert a string with no escaping, turn off the 'Escape' option. Warning: this may create an invalid or unexpected output.";

        [NodeInput]
        public InputString Input { get; } = new InputString("") { Title = "Text:"};
        [NodeInput]
        public InputCheckbox InputDoEscape { get; } = new InputCheckbox(true) { Title = "Escape" };

        public TextNode() { }
        public TextNode(string contents, bool escape = true)
        {
            Input.Contents = contents;
            InputDoEscape.IsChecked = escape;
        }

        static readonly HashSet<char> charsToEscape = new HashSet<char> { '/', '(', ')', '[', ']', '{', '}', '$', '^', '?', '^', '.', '+', '*', '|' };

        protected override string GetValue()
        {
            string result = Input.GetValue();

            if (String.IsNullOrEmpty(result))
            {
                return "";
            }

            if (InputDoEscape.IsChecked)
            {
                result = result.EscapeCharacters(charsToEscape); 
            }

            return result;
        }

        public static TextNode CreateWithContents(string contents)
        {
            string escapedContents = StripUnnecessaryEscapes(contents);

            var result = new TextNode(escapedContents);
            return result;

            string StripUnnecessaryEscapes(string input)
            {
                for(var i = 0; i < input.Length - 1; i++)
                {
                    if(input[i] == '\\'
                        && charsToEscape.Contains(input[i + 1]))
                    {
                        input = input.Remove(i, 1);
                    }
                }
                return input;
            }
        }
    }
}
