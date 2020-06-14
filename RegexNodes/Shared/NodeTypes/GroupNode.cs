using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class GroupNode : Node
    {
        public override string Title => "Group";
        public override string NodeInfo => "Wraps the input node in a group. A capturing or named group can be used later in a backrefence (with the 'Reference' node) or in the  Replacement Regex.";

        [NodeInput]
        protected InputProcedural Input { get; } = new InputProcedural();
        [NodeInput]
        protected InputDropdown InputGroupType { get; } = new InputDropdown(
            "Capturing",
            "Non-capturing",
            "Named",
            "Custom")
        { Title = "Type of group:" };
        [NodeInput]
        protected InputString GroupName { get; } = new InputString("") { Title = "Name:" };
        [NodeInput]
        protected InputString CustomPrefix { get; } = new InputString("") { Title = "Prefix:" };

        public GroupNode()
        {
            GroupName.IsEnabled = (() => InputGroupType.DropdownValue == "Named");
            CustomPrefix.IsEnabled = (() => InputGroupType.DropdownValue == "Custom");
        }

        protected override string GetValue()
        {
            string input = Input.GetValue().RemoveNonCapturingGroup();
            string prefix = "";
            switch (InputGroupType.DropdownValue)
            {
                case "Capturing": prefix = "("; break;
                case "Non-capturing": prefix = "(?:"; break;
                case "Named": prefix = $"(?<{GroupName.GetValue()}>"; break;
                case "Custom": prefix = "(" + CustomPrefix.GetValue(); break;
            };
            //string prefix = (InputGroupType.Value == "Capturing") ? "(" : "(?:";
            //if (input.StartsWith("(?:") && input.EndsWith(")"))
            //{
            //    return UpdateCache(prefix + input.Substring(3));
            //}
            return $"{prefix}{input})";
        }
    }
}
