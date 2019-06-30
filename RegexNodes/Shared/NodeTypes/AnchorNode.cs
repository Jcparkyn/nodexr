using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class AnchorNode : Node
    {
        public override string Title => "Anchor";
        public override string NodeInfo => "Inserts a start-of-line or end-of-line character. Useful for ensuring that your regex only matches if it's at a specific position in a line.";

        static readonly Dictionary<string, string> options = new Dictionary<string, string>
        {
            { "Start of line", "^" },
            { "End of line", "$" },
            { @"Carriage Return", @"\r" },
            { @"LF (\n)", @"\n" },
        };

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { InputLineStart, InputLineEnd, InputWordBoundary };
            }
        }

        protected InputCheckbox InputLineStart { get; set; } = new InputCheckbox(true) { Title = "Start of line" };
        protected InputCheckbox InputLineEnd { get; set; } = new InputCheckbox() { Title = "End of line" };
        protected InputCheckbox InputWordBoundary { get; set; } = new InputCheckbox() { Title = "Word boundary"};


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
