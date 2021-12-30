namespace Nodexr.NodeTypes;
using BlazorNodes.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class OptionalNode : RegexNodeViewModelBase
{
    public override string Title => "Optional";

    public override string NodeInfo => "Makes the input nodes optional. " +
        "This is equivalent to using a Quantifier node with Repetitions: 'Zero or one'.";

    [NodeInput]
    public InputProcedural InputContents { get; } = new InputProcedural()
    {
        Title = "Input",
        Description = "The node or set of nodes that are optional",
    };

    protected override NodeResultBuilder GetValue()
    {
        var builder = new NodeResultBuilder(InputContents.Value);

        string suffix = "";
        string prefix = "";

        //Surround with non-capturing group if necessary
        if (InputContents.ConnectedNode is RegexNodeViewModelBase _node
            && QuantifierNode.RequiresGroupToQuantify(_node))
        {
            prefix += "(?:";
            suffix += ")";
        }

        //Add quantifier
        suffix += "?";

        builder.Prepend(prefix, this);
        builder.Append(suffix, this);
        return builder;
    }
}
