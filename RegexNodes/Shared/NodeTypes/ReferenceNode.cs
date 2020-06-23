using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class ReferenceNode : Node, INode
    {
        public override string Title => "Reference";
        public override string NodeInfo => "Inserts a backreference (or forward-reference if the language supports it) to a captured group, either by name or index.";

        [NodeInput]
        public InputDropdown InputType { get; } = new InputDropdown(
            InputTypes.index,
            InputTypes.name)
        { Title = "Type:" };
        [NodeInput]
        public InputNumber InputIndex { get; } = new InputNumber(1, min: 1) { Title = "Index:" };
        [NodeInput]
        public InputString InputName { get; } = new InputString("") { Title = "Name:" };

        public static class InputTypes
        {
            public const string index = "Index";
            public const string name = "Name";
        }

        public ReferenceNode()
        {
            InputIndex.IsEnabled = (() => InputType.DropdownValue == InputTypes.index);
            InputName.IsEnabled = (() => InputType.DropdownValue == InputTypes.name);
        }

        protected override string GetValue()
        {
            //string prefix = (InputGroupType.Value == "Capturing") ? "(" : "(?:";
            if (InputType.DropdownValue == InputTypes.index)
            {
                return @"\" + InputIndex.InputContents;
            }
            else if (InputType.DropdownValue == InputTypes.name)
            {
                return @"\k<" + InputName.Contents + ">";
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
