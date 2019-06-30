using System.Collections.Generic;
using System.Linq;

namespace RegexNodes.Shared.NodeTypes
{
    public class WhitespaceNode : Node
    {
        public override string Title => "Whitespace";
        public override string NodeInfo => "Matches any of the specified types of whitespace character.";


        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { InputSpace, InputTab, InputCR, InputLF };
            }
        }

        protected InputCheckbox InputSpace { get; set; } = new InputCheckbox(true) { Title = "Space" };
        protected InputCheckbox InputTab { get; set; } = new InputCheckbox(true) { Title = "Tab" };
        protected InputCheckbox InputCR { get; set; } = new InputCheckbox(true) { Title = "Newline (\\r)" };
        protected InputCheckbox InputLF { get; set; } = new InputCheckbox(true) { Title = "Newline (\\n)" };


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
