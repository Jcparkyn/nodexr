using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;

namespace Nodexr.Shared.NodeTypes
{
    public class DecimalNode : Node
    {
        public override string Title => "Decimal";

        public override string NodeInfo => "Matches a decimal/floating point number."; //TODO

        [NodeInput]
        public InputDropdown<SignType> InputSign { get; } = new InputDropdown<SignType>()
        {
            Title = "Sign",
            Description = "Should the number have a sign (+-)?"
        };

        [NodeInput]
        public InputString InputDecimalSeparator { get; } = new InputString(".") { Title = "Decimal Separator(s)" };

        [NodeInput]
        public InputString InputThousandsSeparator { get; } = new InputString("") { Title = "Digit Separator(s)" };

        [NodeInput]
        public InputDropdown<CaptureType> InputCaptureType { get; } = new InputDropdown<CaptureType>(captureTypeDisplayNames)
        {
            Title = "Capture?",
            Description = "Store the result using a capturing group."
        };

        [NodeInput]
        public InputString InputGroupName { get; } = new InputString("") { Title = "Capture Name" };

        [NodeInput]
        public InputCheckbox InputOptionalDecimal { get; } = new InputCheckbox() { Title = "Optional Decimal" };

        [NodeInput]
        public InputCheckbox InputOptionalInteger { get; } = new InputCheckbox() { Title = "Optional Integer" };

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
        }

        public DecimalNode()
        {
            InputGroupName.IsEnabled = (() => InputCaptureType.Value == CaptureType.Named);
        }

        protected override NodeResultBuilder GetValue()
        {

            string signPrefix = InputSign.Value switch
            {
                SignType.Optional => "[-+]?",
                SignType.Compulsory => "[-+]",
                _ => "",
            };

            string decimalSeparator = "[" + InputDecimalSeparator.GetValue() + "]";

            string number;
            bool noThousandsSeparator = string.IsNullOrEmpty(InputThousandsSeparator.GetValue());

            if (noThousandsSeparator)
            {
                number = (InputOptionalDecimal.IsChecked, InputOptionalInteger.IsChecked) switch
                {
                    (false, false) => $@"\d+{decimalSeparator}\d+",
                    (false, true) => $@"\d*{decimalSeparator}\d+",
                    (true, false) => $@"\d+(?:{decimalSeparator}\d+)?",
                    (true, true) => $@"\d*{decimalSeparator}?\d+",
                };
            }
            else
            {
                string thousandsSeparator = InputThousandsSeparator.GetValue();

                number = (InputOptionalDecimal.IsChecked, InputOptionalInteger.IsChecked) switch
                {
                    (false, false) => $@"\d[\d{thousandsSeparator}]*{decimalSeparator}\d+",
                    (false, true) => $@"(?:\d[\d{thousandsSeparator}]*)?{decimalSeparator}\d+",
                    (true, false) => $@"\d[\d{thousandsSeparator}]*(?:{decimalSeparator}\d+)?",
                    (true, true) => $@"(?:\d[\d{thousandsSeparator}]*)?{decimalSeparator}?\d+",
                };
            }

            string contents = signPrefix + number;
            
            string result = InputCaptureType.Value switch
            {
                CaptureType.Capturing => "(" + contents + ")",
                CaptureType.Named => $"(?<{InputGroupName.GetValue()}>{contents})",
                _ => contents,
            };

            return new NodeResultBuilder(result, this);
        }
    }
}
