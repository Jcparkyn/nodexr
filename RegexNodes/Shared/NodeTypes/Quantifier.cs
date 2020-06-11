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
            Repetitions.zeroOrMore,
            Repetitions.oneOrMore,
            Repetitions.zeroOrOne,
            Repetitions.number,
            Repetitions.range) { Title = "Repetitions:" };
        [NodeInput]
        protected InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };
        [NodeInput]
        protected InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "Minimum:" };
        [NodeInput]
        protected InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "Maximum:" };
        [NodeInput]
        protected InputDropdown InputSearchType { get; } = new InputDropdown("Greedy", "Lazy") { Title = "Search type:" };

        private class Repetitions
        {
            public const string zeroOrMore = "Zero or more";
            public const string oneOrMore = "One or more";
            public const string zeroOrOne = "Zero or one";
            public const string number = "Number";
            public const string range = "Range";
        }

        public Quantifier()
        {
            InputNumber.IsEnabled = () => InputCount.DropdownValue == Repetitions.number;
            InputMin.IsEnabled = () => InputCount.DropdownValue == Repetitions.range;
            InputMax.IsEnabled = () => InputCount.DropdownValue == Repetitions.range;
        }

        protected override string GetValue()
        {
            string suffix = "";
            //int min = InputMin.GetValue() ?? 0;
            //int? max = InputMax.GetValue();

            switch (InputCount.DropdownValue)
            {
                case Repetitions.zeroOrMore: suffix = "*"; break;
                case Repetitions.oneOrMore: suffix = "+"; break;
                case Repetitions.zeroOrOne: suffix = "?"; break;
                case Repetitions.number: suffix = $"{{{InputNumber.InputContents}}}"; break;
                case Repetitions.range:
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
