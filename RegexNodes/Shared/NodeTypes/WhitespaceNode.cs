using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace RegexNodes.Shared.NodeTypes
{
    public class WhitespaceNode : Node
    {
        public override string Title => "Whitespace";
        public override string NodeInfo => "Matches any of the specified types of whitespace character.";

        [NodeInput]
        protected InputCheckbox InputSpace { get; } = new InputCheckbox(true) { Title = "Space" };
        [NodeInput]
        protected InputCheckbox InputTab { get; } = new InputCheckbox(true) { Title = "Tab" };
        [NodeInput]
        protected InputCheckbox InputCR { get; } = new InputCheckbox(true) { Title = "Newline (\\r)" };
        [NodeInput]
        protected InputCheckbox InputLF { get; } = new InputCheckbox(true) { Title = "Newline (\\n)" };


        public override string GetValue()
        {
            List<string> charsToAllow = new List<string>();

            if (InputSpace.IsChecked) charsToAllow.Add(" ");
            if (InputTab.IsChecked) charsToAllow.Add("\\t");
            if (InputCR.IsChecked) charsToAllow.Add("\\r");
            if (InputLF.IsChecked) charsToAllow.Add("\\n");

            string charsConverted = string.Join("", charsToAllow);
            if (charsToAllow.Count > 1)
            {
                return UpdateCache("[" + charsConverted + "]");
            }
            else
            {
                return UpdateCache("" + charsConverted);
            }
        }
    }
}
