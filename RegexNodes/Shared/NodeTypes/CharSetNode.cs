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
        protected InputString InputCharacters { get; } = new InputString("a-z") { Title = "Characters:" };
        [NodeInput]
        protected InputCheckbox InputDoInvert { get; } = new InputCheckbox(false) { Title = "Invert"};

        [NodeInput]
        public InputDropdown InputCount { get; } = new InputDropdown(
            Quantifier.Repetitions.one,
            Quantifier.Repetitions.zeroOrMore,
            Quantifier.Repetitions.oneOrMore,
            Quantifier.Repetitions.zeroOrOne,
            Quantifier.Repetitions.number,
            Quantifier.Repetitions.range)
        { Title = "Repetitions:" };
        [NodeInput]
        public InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };
        [NodeInput]
        public InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "Minimum:" };
        [NodeInput]
        public InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "Maximum:" };

        public CharSetNode()
        {
            InputNumber.IsEnabled = () => InputCount.DropdownValue == Quantifier.Repetitions.number;
            InputMin.IsEnabled = () => InputCount.DropdownValue == Quantifier.Repetitions.range;
            InputMax.IsEnabled = () => InputCount.DropdownValue == Quantifier.Repetitions.range;
        }

        protected override string GetValue()
        {
            string charSet = InputCharacters.GetValue();
            if (String.IsNullOrEmpty(charSet))
            {
                return UpdateCache("");
            }

            string prefix = InputDoInvert.IsChecked ? "^" : "";
            string result = "[" + prefix + charSet + "]";

            string suffix = Quantifier.Repetitions.GetSuffix(
                InputCount.DropdownValue,
                InputNumber.InputContents,
                InputMin.GetValue(),
                InputMax.GetValue());

            return UpdateCache(result + suffix);
        }
    }
}
