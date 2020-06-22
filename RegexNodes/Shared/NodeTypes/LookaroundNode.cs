using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class LookaroundNode : Node
    {
        public override string Title => "Lookaround";
        public override string NodeInfo => "Converts the input node into a lookahead or lookbehind.";

        [NodeInput]
        public InputProcedural Input { get; } = new InputProcedural() { Title = "Contents" };
        [NodeInput]
        public InputDropdown InputGroupType { get; } = new InputDropdown(
            "Lookahead",
            "Lookbehind",
            "Negative Lookahead",
            "Negative Lookbehind")
        { Title = "Type:" };

        protected override string GetValue()
        {
            string input = Input.GetValue().RemoveNonCapturingGroup();
            string prefix = "";
            switch (InputGroupType.DropdownValue)
            {
                case "Lookahead": prefix = "(?="; break;
                case "Lookbehind": prefix = "(?<="; break;
                case "Negative Lookahead": prefix = "(?!"; break;
                case "Negative Lookbehind": prefix = "(?<!"; break;
            };
            
            return $"{prefix}{input})";
        }
    }
}
