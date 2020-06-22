using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pidgin;
using RegexNodes.Shared.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static RegexNodes.Shared.RegexParsers.ParsersShared;

namespace RegexNodes.Shared.RegexParsers
{
    public static class GroupParser
    {

        public static Parser<char, GroupNode> ParseGroup =>
            Map((node, contents) => node.WithContents(contents),
                GroupPrefix,
                Rec(() => RegexParser.ParseRegex))
            .Between(Char('('), Char(')'));

        private static Parser<char, GroupNode> GroupPrefix =>
            Char('?').Then(
                OneOf(
                    Char(':').Select(c => CreateWithNonCapturing()),
                    GroupName.Select(name => CreateWithName(name))
                    ))
            .Or(ReturnLazy(CreateWithCapturing));

        private static Parser<char, string> GroupName =>
            OneOf("<'")
            .Then(AnyCharExcept(">'").ManyString())
            .Before(OneOf(">'"));

        private static GroupNode CreateWithCapturing()
        {
            var node = new GroupNode();
            node.InputGroupType.DropdownValue = GroupNode.GroupTypes.capturing;
            return node;
        }

        private static GroupNode CreateWithNonCapturing()
        {
            var node = new GroupNode();
            node.InputGroupType.DropdownValue = GroupNode.GroupTypes.nonCapturing;
            return node;
        }

        private static GroupNode CreateWithName(string name)
        {
            var node = new GroupNode();
            node.InputGroupType.DropdownValue = GroupNode.GroupTypes.named;
            node.GroupName.Contents = name;
            return node;
        }

        private static GroupNode WithContents(this GroupNode node, Node contents)
        {
            node.Input.ConnectedNode = contents;
            return node;
        }
    }
}
