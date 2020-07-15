using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pidgin;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using Nodexr.Shared.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Nodexr.Shared.RegexParsers.ParsersShared;

namespace Nodexr.Shared.RegexParsers
{
    public static class RegexParser
    {
        public static readonly Parser<char, Node> ParseRegex =
            ParseRegexWithoutAlternation
            .Or(Return<Node>(null)) //Empty OrNode options are allowed.
            .WithOptionalAlternation();

        public static Parser<char, Node> ParseRegexWithoutAlternation =>
            ParseSingleNode
            .AtLeastOnce()
            .Select(ConnectNodesInSequence);

        private static Parser<char, Node> ParseSingleNode =>
            TextParser.ParseTextWithOptionalQuantifier
            .Or(
                OneOf(
                    CharSetParser.ParseCharSet.Cast<Node>(),
                    GroupParser.ParseGroup.Cast<Node>(),
                    WildcardParser.ParseWildcard.Cast<Node>(),
                    AnchorParser.ParseAnchor.Cast<Node>(),
                    ParseEscapedWord)
                .WithOptionalQuantifier());

        public static Parser<char, Node> ParseEscapedWord =>
            EscapeChar.Then(ParseSpecialAfterEscape);

        public static Parser<char, Node> ParseSpecialAfterEscape =>
            OneOf(
                UnicodeParser.ParseUnicode.Cast<Node>(),
                ReferenceParser.ParseReference.Cast<Node>(),
                WildcardParser.ParseWildcardAfterEscape.Cast<Node>(),
                AnchorParser.ParseAnchorAfterEscape.Cast<Node>(),
                WhitespaceParser.ParseWhitespaceAfterEscape.Cast<Node>());

        public static Result<char, NodeTree> Parse(string input)
        {
            return OutputParser.ParseOutputNode.Select(BuildNodeTree).Parse(input);
        }

        public static NodeTree BuildNodeTree(Node endNode)
        {
            var builder = new NodeTreeBuilder(endNode);
            return builder.Build();
        }

        private static Node ConnectNodesInSequence(IEnumerable<Node> nodes)
        {
            var endNode = nodes.Aggregate((first, second) =>
            {
                first.ConnectBefore(second);
                return second;
            });

            return endNode;
        }

        private static void ConnectBefore(this Node first, Node second)
        {
            if(second.PreviousNode is null)
            {
                second.PreviousNode = first;
                return;
            }
            else
            {
                first.ConnectBefore((Node)second.PreviousNode);
            }
        }
    }
}
