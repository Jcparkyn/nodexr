using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class Backreference : Node, INode
    {
        public override string Title => "Reference";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { InputType, InputIndex, InputName };
            }
        }

        protected InputDropdown InputType { get; set; } = new InputDropdown(
            "Index",
            "Name")
        { Title = "Type:" };
        protected InputNumber InputIndex { get; set; } = new InputNumber(1, min: 1) { Title = "Index:" };
        protected InputString InputName { get; set; } = new InputString("") { Title = "Name:" };

        public Backreference()
        {
            InputIndex.IsEnabled = (() => InputType.DropdownValue == "Index");
            InputName.IsEnabled = (() => InputType.DropdownValue == "Name");
        }

        public override string GetValue()
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
