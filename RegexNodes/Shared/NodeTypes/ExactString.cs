using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RegexNodes.Shared.NodeTypes
{
    public class ExactString : Node
    {
        public override string Title => "Text";
        public override string NodeInfo => "Inserts text into your Regex which will be interpreted literally, so all special characters are escaped with a backslash. E.g. $25.99? becomes \\$25\\.99\\?" +
            "\nTo insert a string with no escaping, turn off the 'Escape' option. Warning: this may create an invalid or unexpected output.";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { Input, InputDoEscape };
            }
        }

        protected InputString Input { get; set; } = new InputString("") { Title = "Text:"};
        protected InputCheckbox InputDoEscape { get; set; } = new InputCheckbox(true) { Title = "Escape" };

        public ExactString() { }
        public ExactString(string contents)
        {
            Input = new InputString(contents);
        }

        static readonly HashSet<char> charsToEscape = new HashSet<char> { '\\', '/', '(', ')', '[', ']', '{', '}', '$', '^', '?', '^', '.', '+', '*', '|' };

        public override string GetValue()
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
