using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class GroupNode : Node
    {
        public override string Title => "Group";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { Input, InputGroupType, GroupName, CustomPrefix };
            }
        }

        public InputProcedural Input { get; set; } = new InputProcedural();

        public InputDropdown InputGroupType { get; set; } = new InputDropdown(
            "Capturing",
            "Non-capturing",
            "Named",
            "Lookahead",
            "Lookbehind",
            "Negative Lookahead",
            "Negative Lookbehind",
            "Custom")
        { Title = "Type of group:" };

        public InputString GroupName { get; set; } = new InputString("") { Title = "Name:" };
        public InputString CustomPrefix { get; set; } = new InputString("") { Title = "Prefix:" };

        public GroupNode()
        {
            GroupName.IsEnabled = (() => InputGroupType.DropdownValue == "Named");
            CustomPrefix.IsEnabled = (() => InputGroupType.DropdownValue == "Custom");
        }

        public override string GetValue()
        {
            string input = Input.GetValue().RemoveNonCapturingGroup();
            string prefix = "";
            switch (InputGroupType.DropdownValue)
            {
                case "Capturing": prefix = "("; break;
                case "Non-capturing": prefix = "(?:"; break;
                case "Named": prefix = $"(?<{GroupName.GetValue()}>"; break;
                case "Lookahead": prefix = "(?="; break;
                case "Lookbehind": prefix = "(?<="; break;
                case "Negative Lookahead": prefix = "(?!"; break;
                case "Negative Lookbehind": prefix = "(?<!"; break;
                case "Custom": prefix = "(" + CustomPrefix.GetValue(); break;
            };
            //string prefix = (InputGroupType.Value == "Capturing") ? "(" : "(?:";
            //if (input.StartsWith("(?:") && input.EndsWith(")"))
            //{
            //    return UpdateCache(prefix + input.Substring(3));
            //}
            return UpdateCache($"{prefix}{input})");
        }
    }
}
