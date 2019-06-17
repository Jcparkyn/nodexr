using System;
using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class CharSet : Node
    {
        public override string Title => "Character Set";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { InputCharacters, InputDoEscape };
            }
        }

        protected InputString InputCharacters { get; set; } = new InputString("") { Title = "Characters:" };
        protected InputCheckbox InputDoEscape { get; set; } = new InputCheckbox(true) { Title = "Escape?"};


        public CharSet() { }
        public CharSet(string contents)
        {
            InputCharacters = new InputString(contents);
        }

        static readonly HashSet<char> charsToEscape = new HashSet<char> { '/', '(', ')', '[', ']', '{', '}', '$', '^', '?', '^', '.', '+', '*', '|' };


        public override string GetValue()
        {
            string charSet = InputCharacters.GetValue();
            if (String.IsNullOrEmpty(charSet))
            {
                return "";
            }
            if (InputDoEscape.IsChecked)
            {
                charSet = charSet.EscapeCharacters(charsToEscape);
            }
            string result = "[" + charSet + "]";

            return UpdateCache(result);
        }
    }
}
