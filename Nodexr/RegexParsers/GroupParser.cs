using Pidgin;
using Nodexr.NodeTypes;
using Nodexr.Shared.Nodes;
using static Pidgin.Parser;

namespace Nodexr.RegexParsers
{
    public static class GroupParser
    {
        /// <summary>
        /// Parse any type of group. This includes anything in (non-escaped) parentheses.
        /// </summary>
        public static Parser<char, RegexNodeViewModelBase> ParseGroup =>
            Char('?').Then(OneOf(
                LookaroundParser.ParseLookaround.Cast<RegexNodeViewModelBase>(),
                OtherGroup,
                IfElseParser.ParseIfElse.Cast<RegexNodeViewModelBase>()
                ))
            .Or(CaptureGroup)
            .Between(Char('('), Char(')'));

        private static Parser<char, RegexNodeViewModelBase> OtherGroup =>
            Map((node, contents) => node.WithContents(contents),
                NormalGroupPrefix,
                Rec(() => RegexParser.ParseRegex));

        private static Parser<char, RegexNodeViewModelBase> CaptureGroup =>
            Rec(() => RegexParser.ParseRegex)
            .Select(contents =>
                CreateWithType(GroupNode.GroupTypes.capturing)
                .WithContents(contents));

        private static Parser<char, GroupNode> NormalGroupPrefix =>
            OneOf(
                Char(':').Select(_ => CreateWithType(GroupNode.GroupTypes.nonCapturing)),
                Char('>').Select(_ => CreateWithType(GroupNode.GroupTypes.atomic)),
                GroupName.Select(name => CreateWithName(name))
                );

        private static Parser<char, string> GroupName =>
            OneOf("<'")
            .Then(AnyCharExcept(">'").ManyString())
            .Before(OneOf(">'"));

        private static GroupNode CreateWithType(GroupNode.GroupTypes groupType)
        {
            var node = new GroupNode();
            node.InputGroupType.Value = groupType;
            return node;
        }

        private static GroupNode CreateWithName(string name)
        {
            var node = new GroupNode();
            node.InputGroupType.Value = GroupNode.GroupTypes.named;
            node.GroupName.Value = name;
            return node;
        }

        private static RegexNodeViewModelBase WithContents(this GroupNode node, RegexNodeViewModelBase? contents)
        {
            //This group is not needed if it is actually part of an OrNode
            if (contents is OrNode orNode
                && !orNode.Previous.Connected)
            {
                if (node.InputGroupType.Value == GroupNode.GroupTypes.nonCapturing)
                {
                    return orNode;
                }
                else if (node.InputGroupType.Value == GroupNode.GroupTypes.capturing)
                {
                    orNode.InputCapture.Checked = true;
                    return orNode;
                }
            }
            //Otherwise, connect to the previous node.
            node.Input.ConnectedNode = contents;
            return node;
        }
    }
}
