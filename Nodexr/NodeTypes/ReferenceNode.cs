namespace Nodexr.NodeTypes;
using BlazorNodes.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class ReferenceNode : RegexNodeViewModelBase
{
    public override string Title => "Reference";
    public override string NodeInfo => "Inserts a backreference (or forward-reference if the language supports it) to a captured group, either by name or index.";

    [NodeInput]
    public InputDropdown<InputTypes> InputType { get; } = new InputDropdown<InputTypes>()
    { Title = "Type:" };

    [NodeInput]
    public InputNumber InputIndex { get; } = new InputNumber(1, min: 1)
    {
        Title = "Index:",
        Description = "The index (number) of the group to reference."
    };

    [NodeInput]
    public InputString InputName { get; } = new InputString("")
    {
        Title = "Name:",
        Description = "The name of the group to reference (only works for named groups)."
    };

    public enum InputTypes
    {
        Index,
        Name
    }

    public ReferenceNode()
    {
        InputIndex.Enabled = (() => InputType.Value == InputTypes.Index);
        InputName.Enabled = (() => InputType.Value == InputTypes.Name);
    }

    protected override NodeResultBuilder GetValue()
    {
        return new NodeResultBuilder(ValueString(), this);
    }

    private string ValueString()
    {
        return InputType.Value switch
        {
            InputTypes.Index => @"\" + InputIndex.Value,
            InputTypes.Name => @"\k<" + InputName.Value + ">",
            _ => "",
        };
    }
}
