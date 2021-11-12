namespace Nodexr.NodeTypes;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using Nodexr.Utils;
using BlazorNodes.Core;

public class IntegerNode : RegexNodeViewModelBase
{
    public override string Title => "Integer";

    public override string NodeInfo => "Matches an integer number." +
        "\n\nWarning: this node is marked as 'Experimental' because it will not be preserved " +
        "after using the 'Create Link' or 'Edit' buttons.";

    private readonly IntegerRangeGenerator rangeGenerator = new();

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
    public InputCheckbox InputPreferLongest { get; } = new InputCheckbox(true)
    {
        Title = "Prefer longest",
        Description = "If unchecked, the expression may only match part of a longer number. Leaving this checked may slightly reduce performance.",
    };

    public enum LimitType
    {
        Nothing,
        Value,
        Digits,
    }

    public enum SignType
    {
        None,
        Optional,
        Compulsory,
        Negative,
    }

    public IntegerNode()
    {
        InputValueRange.Enabled = () => InputLimitBy.Value == LimitType.Value;
        InputDigitRange.Enabled = () => InputLimitBy.Value == LimitType.Digits;
    }

    protected override NodeResultBuilder GetValue()
    {
        //Expression to match the number, without sign
        string number = InputLimitBy.Value switch
        {
            LimitType.Value => GetIntegerRangeRegex(InputValueRange.Min ?? 0, InputValueRange.Max ?? 0),
            LimitType.Digits => GetQuantifierRangeSuffix() + QuantifierSuffix,
            LimitType.Nothing => "\\d+" + QuantifierSuffix,
            _ => "\\d+" + QuantifierSuffix,
        };
        var builder = new NodeResultBuilder(number, this);

        //Add non-capturing group to make the alternation work
        if (number.Contains('|'))
            builder.AddNonCaptureGroup(this);

        if (InputLeadingZeros.Checked)
            builder.Prepend("0*", this);

        AddSign(builder);

        return builder;
    }

    private string GetQuantifierRangeSuffix()
    {
        int min = InputDigitRange.Min ?? 0;
        return min == InputDigitRange.Max ?
            $"\\d{{{min}}}" :
            $"\\d{{{min},{InputDigitRange.Max}}}";
    }

    private string QuantifierSuffix =>
        InputPreferLongest.Checked ? "" : "?";

    private void AddSign(NodeResultBuilder builder)
    {
        switch (InputSign.Value)
        {
            case SignType.Compulsory:
                builder.Prepend("[-+]", this); break;
            case SignType.Optional:
                builder.Prepend("[-+]?", this); break;
            case SignType.Negative:
                builder.Prepend("-", this); break;
        }
    }

    private string GetIntegerRangeRegex(int min, int max)
    {
        var ranges = rangeGenerator.GenerateRegexRange(min, max);
        if (InputPreferLongest.Checked)
        {
            ranges.Reverse();
        }
        return string.Join('|', ranges);
    }
}
