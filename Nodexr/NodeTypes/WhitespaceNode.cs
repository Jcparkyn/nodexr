using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using static Nodexr.NodeTypes.IQuantifiableNode;

namespace Nodexr.NodeTypes
{
    public class WhitespaceNode : RegexNodeViewModelBase, IQuantifiableNode
    {
        public override string Title => "Whitespace";

        public override string NodeInfo => "Matches any of the specified types of whitespace character." +
            "\nIf 'Invert' is checked, matches everything BUT the specified types of whitespace character.";

        [NodeInput]
        public InputCheckbox InputInvert { get; } = new InputCheckbox(false)
        {
            Title = "Invert",
            Description = "Match everything except the specified types of whitespace.\nThis includes all non-whitespace characters."
        };

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
        {
            Title = "Repetitions:",
            Description = "Apply a quantifier to this node."
        };

        [NodeInput]
        public InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };

        [NodeInput]
        public InputRange InputRange { get; } = new InputRange(0, 1)
        {
            Title = "Amount:",
            Description = "The amount of repetitions to allow. Leave the maximum field blank to allow unlimited repetitions.",
            MinValue = 0,
            AutoClearMax = true,
        };

        public WhitespaceNode()
        {
            bool IsAllWhitespaceUnchecked() => !InputAllWhitespace.Checked;

            InputSpace.Enabled = IsAllWhitespaceUnchecked;
            InputTab.Enabled = IsAllWhitespaceUnchecked;
            InputCR.Enabled = IsAllWhitespaceUnchecked;
            InputLF.Enabled = IsAllWhitespaceUnchecked;

            InputNumber.Enabled = () => InputCount.Value == Reps.Number;
            InputRange.Enabled = () => InputCount.Value == Reps.Range;
        }

        protected override NodeResultBuilder GetValue()
        {
            string suffix = GetSuffix(this);

            var builder = new NodeResultBuilder(ValueString(), this);
            builder.Append(suffix, this);
            return builder;
        }

        private string ValueString()
        {
            bool invert = InputInvert.Checked;

            if (InputAllWhitespace.Checked)
            {
                return invert ? "\\S" : "\\s";
            }

            List<string> charsToAllow = new List<string>();

            if (InputSpace.Checked) charsToAllow.Add(" ");
            if (InputTab.Checked) charsToAllow.Add("\\t");
            if (InputCR.Checked) charsToAllow.Add("\\r");
            if (InputLF.Checked) charsToAllow.Add("\\n");

            string charsConverted = string.Concat(charsToAllow);
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
