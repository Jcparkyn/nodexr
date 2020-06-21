using System;
using System.Collections.Generic;
using RegexNodes.Shared;

namespace RegexNodes.Shared.NodeTypes
{
    public class QuantifierNode : Node
    {
        public override string Title => "Quantifier";
        public override string NodeInfo => "Inserts a quantifier to set the minimum and maximum number of 'repeats' for the inputted node. Leave the 'max' option blank to allow unlimited repeats. 'Greedy' and 'Lazy' search type will attempt to match as many or as few times as possible respectively.";

        [NodeInput]
        public InputProcedural InputContents { get; } = new InputProcedural() { Title = "Input" };
        [NodeInput]
        public InputDropdown InputCount { get; } = new InputDropdown(
            Repetitions.zeroOrMore,
            Repetitions.oneOrMore,
            Repetitions.zeroOrOne,
            Repetitions.number,
            Repetitions.range) { Title = "Repetitions:" };
        [NodeInput]
        public InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };
        [NodeInput]
        public InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "Minimum:" };
        [NodeInput]
        public InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "Maximum:" };
        [NodeInput]
        public InputDropdown InputSearchType { get; } = new InputDropdown(
            SearchModes.greedy,
            SearchModes.lazy,
            SearchModes.possessive) { Title = "Search type:" };

        public class SearchModes
        {
            public const string greedy = "Greedy";
            public const string lazy = "Lazy";
            public const string possessive = "Possessive";
        }

        public class Repetitions
        {
            public const string one = "One";
            public const string zeroOrMore = "Zero or more";
            public const string oneOrMore = "One or more";
            public const string zeroOrOne = "Zero or one";
            public const string number = "Number";
            public const string range = "Range";

            public static string GetSuffix(string mode, int? number = 0, int? min = 0, int? max = 0)
            {
                return mode switch
                {
                    Repetitions.one => "",
                    Repetitions.zeroOrMore => "*",
                    Repetitions.oneOrMore => "+",
                    Repetitions.zeroOrOne => "?",
                    Repetitions.number => $"{{{number ?? 0}}}",
                    Repetitions.range => $"{{{min ?? 0},{max}}}",
                    _ => throw new ArgumentOutOfRangeException(nameof(mode))
                };
            }
        }

        public QuantifierNode()
        {
            InputNumber.IsEnabled = () => InputCount.DropdownValue == Repetitions.number;
            InputMin.IsEnabled = () => InputCount.DropdownValue == Repetitions.range;
            InputMax.IsEnabled = () => InputCount.DropdownValue == Repetitions.range;
        }

        protected override string GetValue()
        {
            string suffix = Repetitions.GetSuffix(
                InputCount.DropdownValue,
                InputNumber.InputContents,
                InputMin.GetValue(),
                InputMax.GetValue());

            if (InputSearchType.DropdownValue == SearchModes.lazy)
            {
                suffix += "?";
            }
            else if (InputSearchType.DropdownValue == SearchModes.possessive)
            {
                suffix += "+";
            }

            string contents = InputContents.GetValue();
            if (!contents.IsSingleRegexChar())
            {
                contents = contents.EnforceGrouped();
            }

            string result = contents + suffix;
            return result;
        }
    }
}
