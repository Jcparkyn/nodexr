using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pidgin;
using RegexNodes.Shared.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static RegexNodes.Shared.RegexParsers.ParsersShared;

namespace RegexNodes.Shared.RegexParsers
{
    public static class RegexParser
    {

        public static readonly Parser<char, Node> ParseRegex =
            ParseSingleNode
            .AtLeastOnce()
            .Select(ConnectNodesInSequence)
            .WithOptionalAlternation();

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
            return ParseRegex.Select(BuildNodeTree).Parse(input);
        }

        public static NodeTree BuildNodeTree(Node endNode)
        {
            const long spacingX = 250, spacingY = 200;
            Vector2L outputPos = new Vector2L(1200, 300);
            NodeTree tree = new NodeTree();
            var output = new OutputNode() { Pos = outputPos };
            output.PreviousNode = endNode;
            tree.AddNode(output);

            AddNodeChildren(output, outputPos);

            return tree;

            void AddNodeChildren(Node parent, Vector2L position)
            {
                var inputs = parent.GetInputsRecursive().OfType<InputProcedural>();
                var children = inputs.Select(input => input.ConnectedNode).OfType<Node>().ToList();

                var pos = new Vector2L(
                    position.x - spacingX,
                    position.y - spacingY * (children.Count - 1) / 2);

                foreach (var child in children)
                {
                    tree.AddNode(child);
                    child.Pos = pos;
                    AddNodeChildren(child, pos);
                    pos = new Vector2L(pos.x, pos.y + spacingY);
                }
            }
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
