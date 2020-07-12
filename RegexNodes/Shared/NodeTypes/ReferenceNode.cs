using System.Collections.Generic;
using RegexNodes.Shared.Nodes;
using RegexNodes.Shared.NodeInputs;

namespace RegexNodes.Shared.NodeTypes
{
    public class ReferenceNode : Node, INode
    {
        public override string Title => "Reference";
        public override string NodeInfo => "Inserts a backreference (or forward-reference if the language supports it) to a captured group, either by name or index.";

        [NodeInput]
        public InputDropdown<InputTypes> InputType { get; } = new InputDropdown<InputTypes>()
        { Title = "Type:" };
        [NodeInput]
        public InputNumber InputIndex { get; } = new InputNumber(1, min: 1) { Title = "Index:" };
        [NodeInput]
        public InputString InputName { get; } = new InputString("") { Title = "Name:" };

        public enum InputTypes
        {
            Index,
            Name
        }

        public ReferenceNode()
        {
            InputIndex.IsEnabled = (() => InputType.Value == InputTypes.Index);
            InputName.IsEnabled = (() => InputType.Value == InputTypes.Name);
        }

        protected override NodeResultBuilder GetValue()
        {
            return new NodeResultBuilder(ValueString(), this);
        }

        private string ValueString()
        {
            return InputType.Value switch
            {
                InputTypes.Index => @"\" + InputIndex.InputContents,
                InputTypes.Name => @"\k<" + InputName.Contents + ">",
                _ => "",
            };
        }
    }
}
