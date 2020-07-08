using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RegexNodes.Shared.NodeTypes.IQuantifiableNode;

namespace RegexNodes.Shared.NodeTypes
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
        public InputCheckbox InputInvert { get; } = new InputCheckbox(false) { Title = "Invert" };
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
        { Title = "Repetitions:" };
        [NodeInput]
        public InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };
        [NodeInput]
        public InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "Minimum:" };
        [NodeInput]
        public InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "Maximum:" };

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

            InputAllowWhitespace.IsEnabled = isCustom;
            InputAllowUnderscore.IsEnabled = isCustom;
            InputAllowUppercase.IsEnabled = isCustom;
            InputAllowLowercase.IsEnabled = isCustom;
            InputAllowDigits.IsEnabled = isCustom;

            InputInvert.IsEnabled = () => InputType.Value != WildcardType.Everything;

            InputNumber.IsEnabled = () => InputCount.Value == Reps.Number;
            InputMin.IsEnabled = () => InputCount.Value == Reps.Range;
            InputMax.IsEnabled = () => InputCount.Value == Reps.Range;
        }

        protected override NodeResultBuilder GetValue()
        {
            bool invert = InputInvert.IsChecked;

            string suffix = GetSuffix(
                InputCount.Value,
                InputNumber.InputContents,
                InputMin.GetValue(),
                InputMax.GetValue());

            string contents = (InputType.Value, invert) switch
            {
                (WildcardType.Everything, _) => ".",
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
            return new NodeResultBuilder(contents + suffix, this);
        }

        private string GetContentsCustom(bool invert)
        {
            string result;

            var inputs = (
                i: invert,
                w: InputAllowWhitespace.IsChecked,
                L: InputAllowUppercase.IsChecked,
                l: InputAllowLowercase.IsChecked,
                d: InputAllowDigits.IsChecked,
                u: InputAllowUnderscore.IsChecked
                );

            result = inputs switch
            {
                //Handle special cases where simplification is possible - when 'invert' is ticked
                (true, false, false, false, false, false) => @"[]",
                (true, false, false, false, true, false) => @"\D",
                (true, true, false, false, false, false) => @"\S",
                (true, false, true, true, true, true) => @"\W",

                //Handle special cases where simplification is possible - when 'invert' is not ticked
                (false, true, true, true, true, true) => ".",
                (false, false, false, false, false, true) => @"_",
                (false, false, false, false, true, false) => @"\d",
                (false, true, false, false, false, false) => @"\s",
                (false, false, true, true, true, true) => @"\w",

                //Handle general case
                _ => GetClassContents(invert: invert, w: inputs.w, L: inputs.L, l: inputs.l, d: inputs.d, u: inputs.u),
            };

            return result;
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
