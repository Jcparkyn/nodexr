namespace Nodexr.NodeTypes;
using BlazorNodes.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class OrNode : RegexNodeViewModelBase
{
    public override string Title => "Or";

    public override string NodeInfo => "Outputs a Regex that will accept any of the given inputs.";

    [NodeInput]
    public InputCheckbox InputCapture { get; } = new InputCheckbox(false)
    {
        Title = "Capture",
        Description = "Store the result using a capturing group."
    };

    [NodeInput]
    public InputCollection Inputs { get; } = new InputCollection("Option");

    public OrNode()
    {
        Inputs.AddItem();
        Inputs.AddItem();
    }

    /// <summary>
    /// Creates an OrNode with the given nodes as inputs.
    /// </summary>
    public OrNode(IEnumerable<INodeOutput<NodeResult>> inputs)
    {
        foreach (var input in inputs)
        {
            Inputs.AddItem(input);
        }
    }

    protected override NodeResultBuilder GetValue()
    {
        var builder = new NodeResultBuilder();
        var inputs = Inputs.Inputs;
        string prefix = InputCapture.Checked ? "(" : "(?:";
        builder.Append(prefix, this);

        if (inputs.Count > 0)
        {
            builder.Append(inputs.First().Value);
            foreach (var input in inputs.Skip(1))
            {
                builder.Append("|", this);
                builder.Append(input.Value);
            }
        }

        builder.Append(")", this);
        return builder;
    }
}
