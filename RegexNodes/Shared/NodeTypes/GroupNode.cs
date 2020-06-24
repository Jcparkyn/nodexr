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
        public InputDropdown<GroupTypes> InputGroupType { get; } = new InputDropdown<GroupTypes>(groupTypeDisplayNames)
        { Title = "Type of group:" };
        [NodeInput]
        public InputString GroupName { get; } = new InputString("") { Title = "Name:" };
        [NodeInput]
        public InputString CustomPrefix { get; } = new InputString("?>") { Title = "Prefix:" };

        public enum GroupTypes
        {
            capturing,
            nonCapturing,
            named,
            atomic,
            custom
        }

        private static readonly Dictionary<GroupTypes, string> groupTypeDisplayNames = new Dictionary<GroupTypes, string>()
        {
            {GroupTypes.capturing, "Capturing" },
            {GroupTypes.nonCapturing, "Non-capturing" },
            {GroupTypes.named, "Named" },
            {GroupTypes.atomic, "Atomic" },
            {GroupTypes.custom, "Custom" }
        };

        public GroupNode()
        {
            GroupName.IsEnabled = (() => InputGroupType.Value == GroupTypes.named);
            CustomPrefix.IsEnabled = (() => InputGroupType.Value == GroupTypes.custom);
        }

        protected override string GetValue()
        {
            string input = Input.GetValue().RemoveNonCapturingGroup();
            string prefix = InputGroupType.Value switch
            {
                GroupTypes.capturing => "(",
                GroupTypes.nonCapturing => "(?:",
                GroupTypes.named => $"(?<{GroupName.GetValue()}>",
                GroupTypes.atomic => $"(?>",
                GroupTypes.custom => "(" + CustomPrefix.GetValue(),
                _ => "",
            };

            return $"{prefix}{input})";
        }
    }
}
