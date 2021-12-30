namespace Nodexr.NodeTypes;
using BlazorNodes.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class CommentNode : RegexNodeViewModelBase
{
    public override string Title => "Comment";
    public override string NodeInfo => "Inserts a comment block, which will be ignored by the Regex engine.";

    [NodeInput]
    protected InputString Input { get; } = new InputString("");

    public CommentNode() { }
    public CommentNode(string contents)
    {
        Input.Value = contents;
    }

    protected override NodeResultBuilder GetValue()
    {
        string result = "(?#" + Input.GetValue()
            //.Replace("(", "")
            .Replace(")", "")
            + ")";

        var builder = new NodeResultBuilder(result, this);
        return builder;
    }
}
