using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class AnchorNode : Node
    {
        public override string Title => "Anchor";
        public override string NodeInfo => "Inserts a start-of-line or end-of-line character. Useful for ensuring that your regex only matches if it's at a specific position in a line.";

        [NodeInput]
        protected InputCheckbox InputLineStart { get; } = new InputCheckbox(true) { Title = "Start of line" };
        [NodeInput]
        protected InputCheckbox InputLineEnd { get; } = new InputCheckbox() { Title = "End of line" };
        [NodeInput]
        protected InputCheckbox InputWordBoundary { get; } = new InputCheckbox() { Title = "Word boundary"};

        public override string GetValue()
        {
            List<string> charsToAllow = new List<string>();

            if (InputWordBoundary.IsChecked) charsToAllow.Add(@"\b");
            if (InputLineEnd.IsChecked) charsToAllow.Add("$");
            if (InputLineStart.IsChecked) charsToAllow.Add("^");

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
