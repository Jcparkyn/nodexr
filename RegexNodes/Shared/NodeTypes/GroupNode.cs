using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class GroupNode : Node
    {
        public override string Title => "Group";
        public override string NodeInfo => "Wraps the input node in a group. A capturing or named group can be used later in a backrefence (with the 'Reference' node) or in the  Replacement Regex.";

        [NodeInput]
        public InputProcedural Input { get; } = new InputProcedural() { Title = "Contents" };
        [NodeInput]
        public InputDropdown InputGroupType { get; } = new InputDropdown(
            GroupTypes.capturing,
            GroupTypes.nonCapturing,
            GroupTypes.named,
            GroupTypes.custom)
        { Title = "Type of group:" };
        [NodeInput]
        public InputString GroupName { get; } = new InputString("") { Title = "Name:" };
        [NodeInput]
        public InputString CustomPrefix { get; } = new InputString("?>") { Title = "Prefix:" };

        public static class GroupTypes
        {
            public const string capturing = "Capturing";
            public const string nonCapturing = "Non-capturing";
            public const string named = "Named";
            public const string custom = "Custom";
        }

        public GroupNode()
        {
            GroupName.IsEnabled = (() => InputGroupType.DropdownValue == GroupTypes.named);
            CustomPrefix.IsEnabled = (() => InputGroupType.DropdownValue == GroupTypes.custom);
        }

        protected override string GetValue()
        {
            string input = Input.GetValue().RemoveNonCapturingGroup();
            string prefix = "";
            switch (InputGroupType.DropdownValue)
            {
                case GroupTypes.capturing: prefix = "("; break;
                case GroupTypes.nonCapturing: prefix = "(?:"; break;
                case GroupTypes.named: prefix = $"(?<{GroupName.GetValue()}>"; break;
                case GroupTypes.custom: prefix = "(" + CustomPrefix.GetValue(); break;
            };

            return $"{prefix}{input})";
        }
    }
}
