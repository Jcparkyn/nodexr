using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class Lookaround : Node
    {
        public override string Title => "Lookaround";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { Input, InputGroupType };
            }
        }

        public InputProcedural Input { get; set; } = new InputProcedural();

        public InputDropdown InputGroupType { get; set; } = new InputDropdown(
            "Lookahead",
            "Lookbehind",
            "Negative Lookahead",
            "Negative Lookbehind")
        { Title = "Type:" };

        public override string GetValue()
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
            
            return UpdateCache($"{prefix}{input})");
        }
    }
}
