using System;
using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class CharSet : Node
    {
        public override string Title => "Character Set";
        public override string NodeInfo => "Inserts a character class containing the characters you specify. You can enter these the same way you would in a normal regex, including ranges (e.g. A-Z).\nThe 'Invert' option creates a negated class by adding a ^ character at the start.";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { InputCharacters, InputDoInvert };
            }
        }

        protected InputString InputCharacters { get; set; } = new InputString("") { Title = "Characters:" };
        protected InputCheckbox InputDoInvert { get; set; } = new InputCheckbox(false) { Title = "Invert"};
        //protected InputCheckbox InputDoEscape { get; set; } = new InputCheckbox(true) { Title = "Escape"};


        public CharSet() { }
        public CharSet(string contents)
        {
            InputCharacters = new InputString(contents);
        }

        //static readonly HashSet<char> charsToEscape = new HashSet<char> { '/', '(', ')', '[', ']', '{', '}', '$', '^', '?', '^', '|' };


        public override string GetValue()
        {
            string charSet = InputCharacters.GetValue();
            if (String.IsNullOrEmpty(charSet))
            {
                UpdateCache("");
            }
            //if (InputDoEscape.IsChecked)
            //{
            //    charSet = charSet.EscapeCharacters(charsToEscape);
            //}
            string prefix = InputDoInvert.IsChecked ? "^" : "";
            string result = "[" + prefix + charSet + "]";

            return UpdateCache(result);
        }
    }
}
