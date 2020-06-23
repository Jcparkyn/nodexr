using System;
using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class CharSetNode : Node
    {
        public override string Title => "Character Set";
        public override string NodeInfo => "Inserts a character class containing the characters you specify. "
            + "You can enter these the same way you would in a normal regex, including ranges (e.g. A-Z).\n"
            + "The 'Invert' option creates a negated class by adding a ^ character at the start.";

        [NodeInput]
        public InputString InputCharacters { get; } = new InputString("a-z") { Title = "Characters:" };
        [NodeInput]
        public InputCheckbox InputDoInvert { get; } = new InputCheckbox(false) { Title = "Invert"};

        [NodeInput]
        public InputDropdown InputCount { get; } = new InputDropdown(
            QuantifierNode.Repetitions.one,
            QuantifierNode.Repetitions.zeroOrMore,
            QuantifierNode.Repetitions.oneOrMore,
            QuantifierNode.Repetitions.zeroOrOne,
            QuantifierNode.Repetitions.number,
            QuantifierNode.Repetitions.range)
        { Title = "Repetitions:" };
        [NodeInput]
        public InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };
        [NodeInput]
        public InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "Minimum:" };
        [NodeInput]
        public InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "Maximum:" };

        public CharSetNode()
        {
            SetupInputEnables();
        }

        void SetupInputEnables()
        {
            InputNumber.IsEnabled = () => InputCount.DropdownValue == QuantifierNode.Repetitions.number;
            InputMin.IsEnabled = () => InputCount.DropdownValue == QuantifierNode.Repetitions.range;
            InputMax.IsEnabled = () => InputCount.DropdownValue == QuantifierNode.Repetitions.range;
        }

        protected override string GetValue()
        {
            string charSet = InputCharacters.GetValue();
            if (String.IsNullOrEmpty(charSet))
            {
                return "";
            }

            string prefix = InputDoInvert.IsChecked ? "^" : "";
            string result = "[" + prefix + charSet + "]";

            string suffix = QuantifierNode.Repetitions.GetSuffix(
                InputCount.DropdownValue,
                InputNumber.InputContents,
                InputMin.GetValue(),
                InputMax.GetValue());

            return result + suffix;
        }
    }
}
