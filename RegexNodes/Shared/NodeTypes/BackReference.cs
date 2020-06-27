using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class Backreference : Node, INode
    {
        public override string Title => "Reference";
        public override string NodeInfo => "Inserts a backreference (or forward-reference if the language supports it) to a captured group, either by name or index.";

        [NodeInput]
        protected InputDropdown InputType { get; } = new InputDropdown(
            "Index",
            "Name")
        { Title = "Type:" };
        [NodeInput]
        protected InputNumber InputIndex { get; } = new InputNumber(1, min: 1) { Title = "Index:" };
        [NodeInput]
        protected InputString InputName { get; } = new InputString("") { Title = "Name:" };

        public Backreference()
        {
            InputIndex.IsEnabled = (() => InputType.DropdownValue == "Index");
            InputName.IsEnabled = (() => InputType.DropdownValue == "Name");
        }

        protected override string GetValue()
        {
            //string prefix = (InputGroupType.Value == "Capturing") ? "(" : "(?:";
            if (InputType.DropdownValue == "Index")
            {
                return @"\" + InputIndex.InputContents;
            }
            else if (InputType.DropdownValue == "Name")
            {
                return @"\k<" + InputName.InputContents + ">";
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
