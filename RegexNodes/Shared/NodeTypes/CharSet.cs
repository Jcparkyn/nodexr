using System;
using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class CharSet : Node
    {
        public override string Title => "Character Set";
        public override string NodeInfo => "Inserts a character class containing the characters you specify. You can enter these the same way you would in a normal regex, including ranges (e.g. A-Z).\nThe 'Invert' option creates a negated class by adding a ^ character at the start.";


        [NodeInput]
        protected InputString InputCharacters { get; } = new InputString("a-z") { Title = "Characters:" };
        [NodeInput]
        protected InputCheckbox InputDoInvert { get; } = new InputCheckbox(false) { Title = "Invert"};

        
        public override string GetValue()
        {
            string charSet = InputCharacters.GetValue();
            if (String.IsNullOrEmpty(charSet))
            {
                return UpdateCache("");
            }

            string prefix = InputDoInvert.IsChecked ? "^" : "";
            string result = "[" + prefix + charSet + "]";

            return UpdateCache(result);
        }
    }
}
