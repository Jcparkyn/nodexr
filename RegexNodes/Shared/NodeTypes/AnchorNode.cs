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
        protected InputDropdown InputAnchorType { get; } = new InputDropdown("Start of line", "End of line", "Word boundary") { Title = "Type of anchor:" };

        protected override string GetValue()
        {
            string result;
            switch (InputAnchorType.DropdownValue.ToLower())
            {
                case "start of line": result = "^"; break;
                case "end of line": result = "$"; break;
                case "word boundary": result = "\\b"; break;
                default: result = ""; break;
            }
            return result;
        }
    }
}
