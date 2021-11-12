namespace Nodexr.NodeTypes;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using Nodexr.Utils;
using BlazorNodes.Core;

public class RecursionNode : RegexNodeViewModelBase
{
    public override string Title => "Recursion";

    public override string NodeInfo => "Matches recursively nested expressions with brackets." +
        "\n'Open Bracket' and 'Close Bracket' can be either single characters, strings of text " +
        "(which are automatically escaped), or full Regex expressions (when 'Regex in Brackets' is checked)." +
        "\nIf 'Contents' is specified, the specified expression will be matched zero or more times between each bracket. " +
        "If 'Contents' is left empty, it will be automatically configured to match anything other than the expressions for the brackets. " +
        "\n\nWarning: only works for .NET regular expressions." +
        "\n\nWarning: this node is marked as 'Experimental' because it will not be preserved " +
        "after using the 'Create Link' or 'Edit' buttons.";

    [NodeInput]
    public InputProcedural InputContents { get; } = new InputProcedural()
    {
        Title = "Contents (Optional)",
        Description = "The expression to match before recursing into the next level. Should not match either of the bracket expressions. " +
        "If left empty, will match anything other than the expressions for the brackets.",
    };

    [NodeInput]
    public InputString InputOpenBracket { get; } = new InputString("(")
    {
        Title = "Open Bracket:",
        Description = "The expression to match before recursing into the next level.",
    };

    [NodeInput]
    public InputString InputCloseBracket { get; } = new InputString(")")
    {
        Title = "Close Bracket:",
        Description = "The expression to match before returning to the previous level.",
    };

    [NodeInput]
    public InputString InputCaptureName { get; } = new InputString("rec")
    {
        Title = "Capture Name",
        Description = "The name of the group to store capture results in. A separate capture is stored for each recursion. " +
        "If left empty, the results will not be captured.",
    };

    [NodeInput]
    public InputString InputGroupName { get; } = new InputString("open")
    {
        Title = "Stack Group Name",
        Description = "The group name to use to keep track of how many brackets have been matched. " +
        "This group will be empty after a successful match.",
    };

    [NodeInput]
    public InputCheckbox InputEnclosingBrackets { get; } = new InputCheckbox(true)
    {
        Title = "Enclosing Brackets",
        Description = "Only match expressions starting and ending with the brackets. Only the inner levels of brackets will be captured.",
    };

    [NodeInput]
    public InputCheckbox InputRegexInBrackets { get; } = new InputCheckbox(false)
    {
        Title = "Regex in Brackets",
        Description = "If unchecked, the expressions for the open and close brackets will be escaped automatically."
    };

    [NodeInput]
    public InputCheckbox InputNoBacktracking { get; } = new InputCheckbox(false)
    {
        Title = "No Backtracking",
        Description = "Can dramatically improve performance in many cases, but will sometimes cause the match to fail."
    };

    protected override NodeResultBuilder GetValue()
    {
        var builder = new NodeResultBuilder();

        string open = InputOpenBracket.Value;
        string close = InputCloseBracket.Value;

        string openEscaped = open;
        string closeEscaped = close;
        if (!InputRegexInBrackets.Checked)
        {
            openEscaped = open.EscapeSpecialCharacters();
            closeEscaped = close.EscapeSpecialCharacters();
        }

        //Get expression for 'anything other than brackets'
        string notBrackets;
        if (open.Length <= 1 && close.Length <= 1)
        {
            //Use inverted CharSet if brackets are single chars
            notBrackets = $"[^{open.EscapeCharacters("[]")}{close.EscapeCharacters("[]")}]";
        }
        else
        {
            //Use negative lookahead if brackets are expressions
            notBrackets = $"(?!{openEscaped}|{closeEscaped}).";
        }

        var betweenRec = InputContents.Connected
            ? InputContents.Value
            : new NodeResult(notBrackets, this);

        string groupName = InputGroupName.Value;
        string captureGroupName = InputCaptureName.Value;

        string openingGroup = InputNoBacktracking.Checked ?
            "(?>" :
            "(?:";

        //Construct output
        builder.Append(openingGroup, this);
        {
            builder.Append($"(?<{groupName}>{openEscaped})", this);
            builder.Append("|", this);
            builder.Append($"(?<{captureGroupName}-{groupName}>{closeEscaped})", this);
            builder.Append("|", this);
            builder.Append(betweenRec);
        }
        builder.Append(")+", this);
        builder.Append($"(?({groupName})(?!))", this);

        if (InputEnclosingBrackets.Checked)
        {
            builder.Prepend(openEscaped, this);
            builder.Append(closeEscaped, this);
        }

        return builder;
    }
}
