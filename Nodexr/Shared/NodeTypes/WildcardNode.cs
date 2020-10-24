using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using static Nodexr.Shared.NodeTypes.IQuantifiableNode;

namespace Nodexr.Shared.NodeTypes
{
    public class WildcardNode : Node, IQuantifiableNode
    {
        public override string Title => "Wildcard";

        public override string NodeInfo => "Matches any of the specified types of character. " +
            "Note: the 'Everything' option will only match newlines if the Regex is in singleline mode. " +
            "\nUse the 'Repetitions' option to add a quantifier with the selected number or range of repetitions " +
            "(If you need a possessive or lazy quantifer, use a Quantifer node instead).";

        [NodeInput]
        public InputDropdown<WildcardType> InputType { get; } = new InputDropdown<WildcardType>(presetDisplayNames) { Title = "Type:" };

        [NodeInput]
        public InputCheckbox InputMatchNewline { get; } = new InputCheckbox(false)
        {
            Title = "Match newlines",
            Description = "Also match newline (\\n) characters"
        };

        [NodeInput]
        public InputCheckbox InputInvert { get; } = new InputCheckbox(false)
        {
            Title = "Invert",
            Description = "Match everything except the specified characters."
        };

        [NodeInput]
        public InputCheckbox InputAllowWhitespace { get; } = new InputCheckbox(false) { Title = "Whitespace" };

        [NodeInput]
        public InputCheckbox InputAllowUppercase { get; } = new InputCheckbox(true) { Title = "Uppercase Letters" };

        [NodeInput]
        public InputCheckbox InputAllowLowercase { get; } = new InputCheckbox(true) { Title = "Lowercase Letters" };

        [NodeInput]
        public InputCheckbox InputAllowDigits { get; } = new InputCheckbox(true) { Title = "Digits" };

        [NodeInput]
        public InputCheckbox InputAllowUnderscore { get; } = new InputCheckbox(true) { Title = "Underscore" };

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

        public enum WildcardType
        {
            Everything,
            WordCharacters,
            Letters,
            Digits,
            Whitespace,
            Custom
        }

        private static readonly Dictionary<WildcardType, string> presetDisplayNames = new Dictionary<WildcardType, string>
        {
            { WildcardType.Everything, "Everything" },
            { WildcardType.WordCharacters, "Word Characters" },
            { WildcardType.Letters, "Letters" },
            { WildcardType.Digits, "Digits" },
            { WildcardType.Whitespace, "Whitespace" },
            { WildcardType.Custom, "Custom" },
        };

        public WildcardNode()
        {
            bool isCustom() => InputType.Value == WildcardType.Custom;

            InputAllowWhitespace.Enabled = isCustom;
            InputAllowUnderscore.Enabled = isCustom;
            InputAllowUppercase.Enabled = isCustom;
            InputAllowLowercase.Enabled = isCustom;
            InputAllowDigits.Enabled = isCustom;

            InputInvert.Enabled = () => InputType.Value != WildcardType.Everything;
            InputMatchNewline.Enabled = () => InputType.Value == WildcardType.Everything;

            InputNumber.Enabled = () => InputCount.Value == Reps.Number;
            InputRange.Enabled = () => InputCount.Value == Reps.Range;
        }

        protected override NodeResultBuilder GetValue()
        {
            bool invert = InputInvert.Checked;

            string suffix = GetSuffix(this);

            string contents = (InputType.Value, invert) switch
            {
                (WildcardType.Everything, _) => InputMatchNewline.Checked ? @"[\s\S]" : ".",
                (WildcardType.WordCharacters, false) => "\\w",
                (WildcardType.WordCharacters, true) => "\\W",
                (WildcardType.Letters, false) => "[a-zA-Z]",
                (WildcardType.Letters, true) => "[^a-zA-Z]",
                (WildcardType.Digits, false) => "\\d",
                (WildcardType.Digits, true) => "\\D",
                (WildcardType.Whitespace, false) => "\\s",
                (WildcardType.Whitespace, true) => "\\S",
                (WildcardType.Custom, _) => GetContentsCustom(invert),
                _ => ".",
            };

            var builder = new NodeResultBuilder(contents, this);
            builder.Append(suffix, this);
            return builder;
        }

        private string GetContentsCustom(bool invert)
        {
            var inputs = (
                i: invert,
                w: InputAllowWhitespace.Checked,
                L: InputAllowUppercase.Checked,
                l: InputAllowLowercase.Checked,
                d: InputAllowDigits.Checked,
                u: InputAllowUnderscore.Checked
                );

            return inputs switch
            {
                //Handle special cases where simplification is possible - when 'invert' is ticked
                (true, false, false, false, false, false) => "[]",
                (true, false, false, false, true, false) => @"\D",
                (true, true, false, false, false, false) => @"\S",
                (true, false, true, true, true, true) => @"\W",

                //Handle special cases where simplification is possible - when 'invert' is not ticked
                (false, true, true, true, true, true) => ".",
                (false, false, false, false, false, true) => "_",
                (false, false, false, false, true, false) => @"\d",
                (false, true, false, false, false, false) => @"\s",
                (false, false, true, true, true, true) => @"\w",

                //Handle general case
                _ => GetClassContents(invert: invert, w: inputs.w, L: inputs.L, l: inputs.l, d: inputs.d, u: inputs.u),
            };
        }

        private string GetClassContents(bool invert, bool w, bool L, bool l, bool d, bool u)
        {
            string result = invert ? "[^" : "[";
            result += w ? "\\s" : "";

            if(L && l && d && u)
            {
                result += "\\w";
            }
            else
            {
                result +=
                    (L ? "A-Z" : "") +
                    (l ? "a-z" : "") +
                    (d ? "\\d" : "") +
                    (u ? "_" : "");
            }
            return result + "]";
        }
    }
}
