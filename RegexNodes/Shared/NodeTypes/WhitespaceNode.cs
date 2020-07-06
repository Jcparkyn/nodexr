using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using static RegexNodes.Shared.NodeTypes.IQuantifiableNode;

namespace RegexNodes.Shared.NodeTypes
{
    public class WhitespaceNode : Node, IQuantifiableNode
    {
        public override string Title => "Whitespace";
        public override string NodeInfo => "Matches any of the specified types of whitespace character." +
            "\nIf 'Invert' is checked, matches everything BUT the specified types of whitespace character.";


        [NodeInput]
        public InputCheckbox InputInvert { get; } = new InputCheckbox(false) { Title = "Invert" };
        [NodeInput]
        public InputCheckbox InputAllWhitespace { get; } = new InputCheckbox(true) { Title = "All Whitespace" };
        [NodeInput]
        public InputCheckbox InputSpace { get; } = new InputCheckbox(true) { Title = "Space" };
        [NodeInput]
        public InputCheckbox InputTab { get; } = new InputCheckbox(true) { Title = "Tab" };
        [NodeInput]
        public InputCheckbox InputCR { get; } = new InputCheckbox(true) { Title = "Newline (\\r)" };
        [NodeInput]
        public InputCheckbox InputLF { get; } = new InputCheckbox(true) { Title = "Newline (\\n)" };

        [NodeInput]
        public InputDropdown<Reps> InputCount { get; } = new InputDropdown<Reps>(displayNames)
        { Title = "Repetitions:" };
        [NodeInput]
        public InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };
        [NodeInput]
        public InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "Minimum:" };
        [NodeInput]
        public InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "Maximum:" };

        public WhitespaceNode()
        {
            bool IsAllWhitespaceUnchecked() => !InputAllWhitespace.IsChecked;

            InputSpace.IsEnabled = IsAllWhitespaceUnchecked;
            InputTab.IsEnabled = IsAllWhitespaceUnchecked;
            InputCR.IsEnabled = IsAllWhitespaceUnchecked;
            InputLF.IsEnabled = IsAllWhitespaceUnchecked;

            InputNumber.IsEnabled = () => InputCount.Value == Reps.Number;
            InputMin.IsEnabled = () => InputCount.Value == Reps.Range;
            InputMax.IsEnabled = () => InputCount.Value == Reps.Range;
        }

        protected override NodeResultBuilder GetValue()
        {
            string suffix = GetSuffix(
                InputCount.Value,
                InputNumber.InputContents,
                InputMin.GetValue(),
                InputMax.GetValue());

            return new NodeResultBuilder(ValueString() + suffix, this);
        }

        private string ValueString()
        {
            bool invert = InputInvert.IsChecked;
            
            if (InputAllWhitespace.IsChecked)
            {
                return invert ? "\\S" : "\\s";
            }

            List<string> charsToAllow = new List<string>();

            if (InputSpace.IsChecked) charsToAllow.Add(" ");
            if (InputTab.IsChecked) charsToAllow.Add("\\t");
            if (InputCR.IsChecked) charsToAllow.Add("\\r");
            if (InputLF.IsChecked) charsToAllow.Add("\\n");

            string charsConverted = string.Join("", charsToAllow);
            if (invert)
            {
                return "[^" + charsConverted + "]";
            }
            else if (charsToAllow.Count > 1)
            {
                return "[" + charsConverted + "]";
            }
            else
            {
                return "" + charsConverted;
            }
        }
    }
}
