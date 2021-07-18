using System.Collections.Generic;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using BlazorNodes.Core;

namespace Nodexr.NodeTypes
{
    public class GroupNode : RegexNodeViewModelBase
    {
        public override string Title => "Group";
        public override string NodeInfo => "Wraps the input node in a group. A capturing or named group can be used later in a backrefence (with the 'Reference' node) or in the  Replacement Regex.";

        [NodeInput]
        public InputProcedural Input { get; } = new InputProcedural()
        {
            Title = "Contents",
            Description = "The node or set of nodes that will be surrounded by a group."
        };

        [NodeInput]
        public InputDropdown<GroupTypes> InputGroupType { get; } = new InputDropdown<GroupTypes>(groupTypeDisplayNames)
        { Title = "Type of group:" };

        [NodeInput]
        public InputString GroupName { get; } = new InputString("") { Title = "Name:" };

        [NodeInput]
        public InputString CustomPrefix { get; } = new InputString("?") { Title = "Prefix:" };

        public enum GroupTypes
        {
            capturing,
            nonCapturing,
            named,
            atomic,
            custom
        }

        private static readonly Dictionary<GroupTypes, string> groupTypeDisplayNames = new()
        {
            { GroupTypes.capturing, "Capturing" },
            { GroupTypes.nonCapturing, "Non-capturing" },
            { GroupTypes.named, "Named" },
            { GroupTypes.atomic, "Atomic" },
            { GroupTypes.custom, "Custom" }
        };

        public GroupNode()
        {
            GroupName.Enabled = (() => InputGroupType.Value == GroupTypes.named);
            CustomPrefix.Enabled = (() => InputGroupType.Value == GroupTypes.custom);
        }

        protected override NodeResultBuilder GetValue()
        {
            var builder = new NodeResultBuilder(Input.Value);

            if (Input.ConnectedNode is OrNode
                || Input.ConnectedNode is IntegerNode
            )
            {
                builder.StripNonCaptureGroup();
            }

            string prefix = InputGroupType.Value switch
            {
                GroupTypes.capturing => "(",
                GroupTypes.nonCapturing => "(?:",
                GroupTypes.named => $"(?<{GroupName.GetValue()}>",
                GroupTypes.atomic => "(?>",
                GroupTypes.custom => "(" + CustomPrefix.GetValue(),
                _ => "",
            };

            builder.Prepend(prefix, this);
            builder.Append(")", this);
            return builder;
        }
    }
}
