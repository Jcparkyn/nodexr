using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RegexNodes.Shared.NodeTypes
{
    public class TextNode : Node
    {
        public override string Title => "Text";
        public override string NodeInfo => "Inserts text into your Regex which will be interpreted literally, so all special characters are escaped with a backslash. E.g. $25.99? becomes \\$25\\.99\\?" +
            "\nTo insert a string with no escaping, turn off the 'Escape' option. Warning: this may create an invalid or unexpected output.";

        [NodeInput]
        protected InputString Input { get; } = new InputString("") { Title = "Text:"};
        [NodeInput]
        protected InputCheckbox InputDoEscape { get; } = new InputCheckbox(true) { Title = "Escape" };

        public TextNode() { }
        public TextNode(string contents = "")
        {
            Input.InputContents = contents;
        }

        static readonly HashSet<char> charsToEscape = new HashSet<char> { '\\', '/', '(', ')', '[', ']', '{', '}', '$', '^', '?', '^', '.', '+', '*', '|' };

        protected override string GetValue()
        {
            string result = Input.GetValue();

            if (String.IsNullOrEmpty(result))
            {
                CachedValue = null;
                return null;
            }

            if (InputDoEscape.IsChecked)
            {
                result = result.EscapeCharacters(charsToEscape); 
            }

            return UpdateCache(result);
        }
    }
}
