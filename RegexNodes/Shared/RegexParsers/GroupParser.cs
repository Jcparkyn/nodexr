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
        public static readonly Parser<char, GroupNode> ParseGroup =
            Char('(')
            .Then(Rec(() => RegexParser.ParseRegex))
            .Before(Char(')'))
            .Select(contents =>
            {
                var group = new GroupNode();
                group.Input.ConnectedNode = contents;
                return group;
            });
    }
}
