namespace Nodexr.NodeTypes;
using Nodexr.Utils;
using BlazorNodes.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class TextNode : RegexNodeViewModelBase
{
    public override string Title => "Text";

    public override string NodeInfo => "Inserts text into your Regex which will be interpreted literally, " +
        "so all special characters are escaped with a backslash. E.g. $25.99? becomes \\$25\\.99\\?" +
        "\nNote: Backslash characters (\\), and the character immediately following them, are not escaped." +
        "\nTo insert a string with no escaping, turn off the 'Escape' option. Warning: this may create an invalid or unexpected output.";

    [NodeInput]
    public InputString Input { get; } = new InputString("")
    {
        Title = "Text:",
        Description = "The text to match."
    };

    [NodeInput]
    public InputCheckbox InputEscapeSpecials { get; } = new InputCheckbox(true)
    {
        Title = "Escape Specials",
        Description = "Should special characters (e.g. ^$?+) be escaped automatically?"
    };

    [NodeInput]
    public InputCheckbox InputEscapeBackslash { get; } = new InputCheckbox(false)
    {
        Title = "Escape Backslash",
        Description = "Should backslashes also be escaped automatically?"
    };

    private const string CharsToEscape = "()[]{}$^?.+*|";

    protected override NodeResultBuilder GetValue()
    {
        string result = Input.GetValue() ?? "";

        if (InputEscapeBackslash.Checked)
        {
            result = result.EscapeCharacters("\\");
        }
        if (InputEscapeSpecials.Checked)
        {
            result = result.EscapeCharacters(CharsToEscape);
        }

        return new NodeResultBuilder(result, this);
    }

    public static TextNode CreateWithContents(string contents)
    {
        string escapedContents = StripUnnecessaryEscapes(contents);

        var result = new TextNode();
        result.Input.Value = escapedContents;
        return result;

        static string StripUnnecessaryEscapes(string input)
        {
            for (int i = 0; i < input.Length - 1; i++)
            {
                if (input[i] == '\\'
                    && CharsToEscape.Contains(input[i + 1]))
                {
                    //Remove the backslash. This automatically causes the next character to be skipped.
                    input = input.Remove(i, 1);
                }
            }
            return input;
        }
    }
}
