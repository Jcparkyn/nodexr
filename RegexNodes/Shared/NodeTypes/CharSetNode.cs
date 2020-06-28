using System;
using System.Collections.Generic;
using static RegexNodes.Shared.NodeTypes.IQuantifiableNode;

namespace RegexNodes.Shared.NodeTypes
{
    public class CharSetNode : Node, IQuantifiableNode
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
        public InputDropdown<Reps> InputCount { get; } = new InputDropdown<Reps>(displayNames)
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
            InputNumber.IsEnabled = () => InputCount.Value == Reps.Number;
            InputMin.IsEnabled = () => InputCount.Value == Reps.Range;
            InputMax.IsEnabled = () => InputCount.Value == Reps.Range;
        }

        protected override NodeResultBuilder GetValue()
        {

            string charSet = InputCharacters.GetValue();

            string prefix = InputDoInvert.IsChecked ? "^" : "";
            string result = "[" + prefix + charSet + "]";

            string suffix = GetSuffix(
                InputCount.Value,
                InputNumber.InputContents,
                InputMin.GetValue(),
                InputMax.GetValue());

            var builder = new NodeResultBuilder();
            builder.Append(result + suffix, this);
            return builder;
        }
    }
}
