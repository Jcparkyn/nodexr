namespace Nodexr.NodeTypes;
using BlazorNodes.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class DecimalNode : RegexNodeViewModelBase
{
    public override string Title => "Decimal";

    public override string NodeInfo => "Matches a decimal/floating point number. " +
        "\nThe digit separator will not enforce that digits are strictly in groups of three, " +
        "but the match will never start with a separator." +
        "\nIf 'Optional Decimal' and 'Optional Integer' are both checked, the expression will " +
        "only match when at least one of them is found." +
        "\n\nWarning: this node is marked as 'Experimental' because it will not be preserved " +
        "after using the 'Create Link' or 'Edit' buttons.";

    [NodeInput]
    public InputDropdown<SignType> InputSign { get; } = new InputDropdown<SignType>()
    {
        Title = "Sign",
        Description = "Should the number have a sign (+-)?"
    };

    [NodeInput]
    public InputString InputDecimalSeparator { get; } = new InputString(".")
    {
        Title = "Decimal Separator(s)",
        Description = "Character to separate the integer part of the number from the decimal. " +
            "If more than one character is entered, the expression will match any one of them."
    };

    [NodeInput]
    public InputString InputThousandsSeparator { get; } = new InputString("")
    {
        Title = "Digit Separator(s)",
        Description = "Character to separate groups of digits in the integer part of the number. Leave blank for no separator."
    };

    [NodeInput]
    public InputCheckbox InputOptionalDecimal { get; } = new InputCheckbox()
    {
        Title = "Optional Decimal",
        Description = "If checked, numbers with no decimal part (e.g. 10) are matched."
    };

    [NodeInput]
    public InputCheckbox InputOptionalInteger { get; } = new InputCheckbox()
    {
        Title = "Optional Integer",
        Description = "If checked, numbers with no integer part (e.g. .5) are matched."
    };

    public enum SignType
    {
        None,
        Optional,
        Compulsory,
    }

    protected override NodeResultBuilder GetValue()
    {
        string signPrefix = InputSign.Value switch
        {
            SignType.Optional => "[-+]?",
            SignType.Compulsory => "[-+]",
            _ => "",
        };

        //decimal separator should fall back to \. if left empty.
        string decimalSeparator = string.IsNullOrEmpty(InputDecimalSeparator.GetValue()) ?
            "\\." :
            "[" + InputDecimalSeparator.GetValue() + "]";

        string number; //Expression to match the number, without sign

        bool noThousandsSeparator = string.IsNullOrEmpty(InputThousandsSeparator.GetValue());
        if (noThousandsSeparator)
        {
            number = (InputOptionalDecimal.Checked, InputOptionalInteger.Checked) switch
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

            number = (InputOptionalDecimal.Checked, InputOptionalInteger.Checked) switch
            {
                (false, false) => $@"\d[\d{thousandsSeparator}]*{decimalSeparator}\d+",
                (false, true) => $@"(?:\d[\d{thousandsSeparator}]*)?{decimalSeparator}\d+",
                (true, false) => $@"\d[\d{thousandsSeparator}]*(?:{decimalSeparator}\d+)?",
                (true, true) => $@"(?:\d[\d{thousandsSeparator}]*)?{decimalSeparator}?\d+",
            };
        }

        return new NodeResultBuilder(signPrefix + number, this);
    }
}
