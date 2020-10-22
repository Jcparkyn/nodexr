using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using System.Linq;

namespace Nodexr.Shared.NodeTypes
{
    public class IntegerNode : Node
    {
        public override string Title => "Integer";

        public override string NodeInfo => "Matches an integer number." +
            "\n\nWarning: this node is marked as 'Experimental' because it will not be preserved " +
            "after using the 'Create Link' or 'Edit' buttons.";

        private readonly IntegerRangeGenerator rangeGenerator = new IntegerRangeGenerator();

        [NodeInput]
        public InputDropdown<LimitType> InputLimitBy { get; } = new InputDropdown<LimitType>()
        {
            Title = "Limit By:",
            Description = "Method to determine which integers should be matched",
            Value = LimitType.Nothing,
        };

        [NodeInput]
        public InputRange InputValueRange { get; } = new InputRange(0, 99)
        {
            Title = "Range of values:",
            Description = "Numbers within this range (inclusive) will be matched.",
            AutoClearMax = false,
            MinValue = 0,
        };

        [NodeInput]
        public InputRange InputDigitRange { get; } = new InputRange(1, null)
        {
            Title = "Number of digits:",
            Description = "Integers with this many digits will be matched. " +
            "Add other criteria before and after the number to prevent parts of larger numbers from being matched.",
            AutoClearMax = true,
            MinValue = 0,
        };

        [NodeInput]
        public InputDropdown<SignType> InputSign { get; } = new InputDropdown<SignType>()
        {
            Title = "Sign:",
            Description = "Should the number have a sign (+-)?"
        };

        [NodeInput]
        public InputCheckbox InputLeadingZeros { get; } = new InputCheckbox(false)
        {
            Title = "Leading zeros?",
            Description = "Allow leading zeros in the number?",
        };

        [NodeInput]
        public InputDropdown<CaptureType> InputCaptureType { get; } = new InputDropdown<CaptureType>(captureTypeDisplayNames)
        {
            Title = "Capture?",
            Description = "Store the result using a capturing group?"
        };

        [NodeInput]
        public InputString InputGroupName { get; } = new InputString("int")
        {
            Title = "Capture Name:",
            Description = "The name to give to the captured group."
        };

        public enum LimitType
        {
            Nothing,
            Value,
            Digits,
        }

        public enum CaptureType
        {
            NonCapturing,
            Capturing,
            Named,
        }

        private static readonly Dictionary<CaptureType, string> captureTypeDisplayNames = new Dictionary<CaptureType, string>()
        {
            {CaptureType.NonCapturing, "Don't Capture" },
            {CaptureType.Capturing, "Capture" },
            {CaptureType.Named, "Named Capture" },
        };

        public enum SignType
        {
            None,
            Optional,
            Compulsory,
            Negative,
        }

        public IntegerNode()
        {
            InputGroupName.IsEnabled = (() => InputCaptureType.Value == CaptureType.Named);
            InputValueRange.IsEnabled = () => InputLimitBy.Value == LimitType.Value;
            InputDigitRange.IsEnabled = () => InputLimitBy.Value == LimitType.Digits;
        }

        public bool IsSingleGroup()
        {
            return InputCaptureType.Value != CaptureType.NonCapturing;
        }

        protected override NodeResultBuilder GetValue()
        {
            //Expression to match the number, without sign
            string number = InputLimitBy.Value switch
            {
                LimitType.Value => GetIntegerRangeRegex(InputValueRange.Min ?? 0, InputValueRange.Max ?? 0),
                LimitType.Digits => (InputDigitRange.Min ?? 0) == InputDigitRange.Max ?
                    $"\\d{{{InputDigitRange.Min}}}" :
                    $"\\d{{{InputDigitRange.Min ?? 0},{InputDigitRange.Max}}}",
                LimitType.Nothing => "\\d+",
                _ => "\\d+",
            };

            if (InputLeadingZeros.IsChecked)
                number = "0*" + number;

            var builder = new NodeResultBuilder(number, this);

            switch (InputSign.Value)
            {
                case SignType.Compulsory:
                    builder.Prepend("[-+]", this); break;
                case SignType.Optional:
                    builder.Prepend("[-+]?", this); break;
                case SignType.Negative:
                    builder.Prepend("-", this); break;
            }

            switch (InputCaptureType.Value)
            {
                case CaptureType.Capturing:
                    builder.Prepend("(", this);
                    builder.Append(")", this);
                    break;
                case CaptureType.Named:
                    builder.Prepend($"(?<{InputGroupName.GetValue()}>", this);
                    builder.Append(")", this);
                    break;
            }

            return builder;
        }

        private string GetIntegerRangeRegex(int min, int max)
        {
            var ranges = rangeGenerator.GenerateRegexRange(min, max);
            return JoinRegexRanges(ranges);
        }

        private string JoinRegexRanges(IEnumerable<string> ranges)
        {
            string regex = string.Join('|', ranges);

            bool needsSubGroup =
                ranges.Count() > 1 &&
                (HasPrefix() || InputCaptureType.Value == CaptureType.NonCapturing);

            if (needsSubGroup)
            {
                regex = "(?:" + regex + ")";
            }
            return regex;
        }

        private bool HasPrefix() => InputLeadingZeros.IsChecked || InputSign.Value != SignType.None;
    }
}
