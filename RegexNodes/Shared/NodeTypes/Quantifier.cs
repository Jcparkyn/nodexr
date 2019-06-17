using System.Collections.Generic;
using RegexNodes.Shared;

namespace RegexNodes.Shared.NodeTypes
{
    public class Quantifier : Node, INode
    {
        public override string Title => "Quantifier";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { InputNode, InputMin, InputMax, InputSearchType };
            }
        }

        protected InputProcedural InputNode { get; set; } = new InputProcedural() { Title = "Input" };
        protected InputNumber InputMin { get; set; } = new InputNumber(0, min: 0) { Title = "minimum:" };
        protected InputNumber InputMax { get; set; } = new InputNumber(1, min: 0) { Title = "maximum:" };
        protected InputDropdown InputSearchType { get; set; } = new InputDropdown("Greedy", "Lazy") { Title = "search type:" };

        public override string GetValue()
        {
            string suffix;
            int min = InputMin.GetValue() ?? 0;
            int? max = InputMax.GetValue();

            if (min == 0 && max == 1)
            {
                suffix = "?";
            }
            else if (min == 0 && max == null)
            {
                suffix = "*";
            }
            else if (min == 1 && max == null)
            {
                suffix = "+";
            }
            else if (min == max)
            {
                suffix = "{" + min + "}";
            }
            //else if (max == 0)
            else
            {
                suffix = $"{{{min},{max}}}";
            }

            if (InputSearchType.DropdownValue == "Lazy" && min != max)
            {
                suffix += "?";
            }
            string result = InputNode.GetValue().EnforceGrouped() + suffix;
            return UpdateCache(result);
        }
    }
}
