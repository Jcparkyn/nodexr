using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace RegexNodes.Shared.NodeTypes
{
    public class WhitespaceNode : Node
    {
        public override string Title => "Whitespace";
        public override string NodeInfo => "Matches any of the specified types of whitespace character." +
            "\nIf 'Invert' is checked, matches everything BUT the specified types of whitespace character.";


        [NodeInput]
        public InputCheckbox InputInvert { get; } = new InputCheckbox(false) { Title = "Invert" };
        [NodeInput]
        public InputCheckbox InputAllWhitespace { get; } = new InputCheckbox(true) { Title = "All Whitespace" };
        [NodeInput]
        public InputCheckbox InputSpace { get; } = new InputCheckbox(true) { Title = "Space" };
        [NodeInput]
        public InputCheckbox InputTab { get; } = new InputCheckbox(true) { Title = "Tab" };
        [NodeInput]
        public InputCheckbox InputCR { get; } = new InputCheckbox(true) { Title = "Newline (\\r)" };
        [NodeInput]
        public InputCheckbox InputLF { get; } = new InputCheckbox(true) { Title = "Newline (\\n)" };

        public WhitespaceNode()
        {
            bool IsAllWhitespaceUnchecked() => !InputAllWhitespace.IsChecked;

            InputSpace.IsEnabled = IsAllWhitespaceUnchecked;
            InputTab.IsEnabled = IsAllWhitespaceUnchecked;
            InputCR.IsEnabled = IsAllWhitespaceUnchecked;
            InputLF.IsEnabled = IsAllWhitespaceUnchecked;
        }

        protected override string GetValue()
        {
            bool invert = InputInvert.IsChecked;
            
            if (InputAllWhitespace.IsChecked)
            {
                return invert ? "\\S" : "\\s";
            }

            List<string> charsToAllow = new List<string>();

            if (InputSpace.IsChecked) charsToAllow.Add(" ");
            if (InputTab.IsChecked) charsToAllow.Add("\\t");
            if (InputCR.IsChecked) charsToAllow.Add("\\r");
            if (InputLF.IsChecked) charsToAllow.Add("\\n");

            string charsConverted = string.Join("", charsToAllow);
            if (invert)
            {
                return "[^" + charsConverted + "]";
            }
            else if (charsToAllow.Count > 1)
            {
                return "[" + charsConverted + "]";
            }
            else
            {
                return "" + charsConverted;
            }
        }
    }
}
