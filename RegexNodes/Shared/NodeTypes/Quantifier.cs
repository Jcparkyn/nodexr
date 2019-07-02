using System.Collections.Generic;
using RegexNodes.Shared;

namespace RegexNodes.Shared.NodeTypes
{
    public class Quantifier : Node, INode
    {
        public override string Title => "Quantifier";
        public override string NodeInfo => "Inserts a quantifier to set the minimum and maximum number of 'repeats' for the inputted node. Leave the 'max' option blank to allow unlimited repeats. 'Greedy' and 'Lazy' search type will attempt to match as many or as few times as possible respectively.";

        [NodeInput]
        protected InputProcedural InputNode { get; } = new InputProcedural() { Title = "Input" };
        [NodeInput]
        protected InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "minimum:" };
        [NodeInput]
        protected InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "maximum:" };
        [NodeInput]
        protected InputDropdown InputSearchType { get; } = new InputDropdown("Greedy", "Lazy") { Title = "search type:" };

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
