using System.Collections.Generic;
using System.Linq;

namespace RegexNodes.Shared.NodeTypes
{
    public class WhitespaceNode : Node
    {
        public override string Title => "Whitespace";
        public override string NodeInfo => "Matches any of the specified types of whitespace character.";

        static readonly Dictionary<string, string> options = new Dictionary<string, string>
        {
            { @"Space", " " },
            { @"Tab", @"\t" },
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

        protected InputDropdown Input { get; set; } = new InputDropdown(options.Keys.ToArray()) { Title = "Character:" };

        public override string GetValue()
        {
            string prefix = options[Input.DropdownValue];
            return UpdateCache(prefix);
        }
    }
}
