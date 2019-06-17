using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class AnchorNode : Node, INode
    {
        public override string Title => "String Anchor";

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
                return new List<INodeInput> { Input };
            }
        }

        public InputDropdown Input { get; set; } = new InputDropdown(options.Keys.ToArray()) { Title = "Character:" };

        public override string GetValue()
        {
            string prefix = options[Input.DropdownValue];
            return UpdateCache(prefix);
        }
    }
}
