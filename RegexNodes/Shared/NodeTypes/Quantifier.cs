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
        protected InputDropdown InputCount { get; } = new InputDropdown(
            "Zero or more",
            "One or more",
            "Zero or one",
            "Number",
            "Range") { Title = "Repetitions:" };
        [NodeInput]
        protected InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };
        [NodeInput]
        protected InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "Minimum:" };
        [NodeInput]
        protected InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "Maximum:" };
        [NodeInput]
        protected InputDropdown InputSearchType { get; } = new InputDropdown("Greedy", "Lazy") { Title = "Search type:" };

        public Quantifier()
        {
            InputNumber.IsEnabled = () => InputCount.DropdownValue == "Number";
            InputMin.IsEnabled = () => InputCount.DropdownValue == "Range";
            InputMax.IsEnabled = () => InputCount.DropdownValue == "Range";
        }

        protected override string GetValue()
        {
            string suffix = "";
            //int min = InputMin.GetValue() ?? 0;
            //int? max = InputMax.GetValue();

            switch (InputCount.DropdownValue)
            {
                case "Zero or more": suffix = "*"; break;
                case "One or more": suffix = "+"; break;
                case "Zero or one": suffix = "?"; break;
                case "Number": suffix = $"{{{InputNumber.InputContents}}}"; break;
                case "Range":
                    int min = InputMin.GetValue() ?? 0;
                    int? max = InputMax.GetValue();
                    suffix = $"{{{min},{max}}}";
                    break;
            }

            if (InputSearchType.DropdownValue == "Lazy")
            {
                suffix += "?";
            }

            string contents = InputNode.GetValue();
            if (!contents.IsSingleRegexChar())
            {
                contents = contents.EnforceGrouped();
            }

            string result = contents + suffix;
            return UpdateCache(result);
        }
    }
}
