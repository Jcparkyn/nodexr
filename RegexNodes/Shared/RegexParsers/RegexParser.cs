using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pidgin;
using RegexNodes.Shared.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace RegexNodes.Shared.RegexParsers
{
    public class RegexParser
    {
        private static readonly Parser<char, char> LBracket = Char('[');
        private static readonly Parser<char, char> RBracket = Char(']');
        private static readonly Parser<char, char> EscapeChar = Char('\\');
        private static readonly Parser<char, char> NotEscapeChar = AnyCharExcept('\\');

        private static readonly Parser<char, string> SingleNonSpecialRegexChar =
            AnyCharExcept("\\|?*+()[{")
                .Select(c => c.ToString())
            .Or(EscapeChar
                .Then(Any)
                .Select(c => "\\" + c)
                );

        private static readonly Parser<char, string> NonSpecialRegexPhrase =
            SingleNonSpecialRegexChar
            .AtLeastOnceString();

        public static readonly Parser<char, string> ValidCharSetChar =
            AnyCharExcept('\\', ']').Select(c => c.ToString())
            .Or(EscapeChar
                .Then(Any)
                .Select(c => "\\" + c)
                );

        //TODO: support negated sets
        private static readonly Parser<char, string> CharSetContents =
            ValidCharSetChar
            .AtLeastOnceString();

        public static readonly Parser<char, TextNode> ParseTextNode =
            NonSpecialRegexPhrase
            .Select(text => TextNode.CreateFromContents(text));

        public static readonly Parser<char, CharSetNode> ParseCharSet =
            LBracket.Then(CharSetContents).Before(RBracket)
                .Select(contents =>
                    new CharSetNode(contents));

        public static Parser<char, Node> ParseRegex =>
            ParseRegexChunk
            .Many()
            .Select(ConnectNodesInSequence);


        public static readonly Parser<char, Node> ParseRegexChunk =
            ParseCharSet.Cast<Node>()
            .Or(ParseTextNode.Cast<Node>())
            .Or(Rec(() => ParseGroup).Cast<Node>());

        public static readonly Parser<char, GroupNode> ParseGroup =
            Char('(')
            .Then(ParseRegex)
            .Before(Char(')'))
            .Select(contents =>
            {
                var group = new GroupNode();
                group.Input.ConnectedNode = contents;
                return group;
            });

        //private static readonly Parser<>

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
            output.PreviousNode.ConnectedNode = endNode;
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
            return nodes.Aggregate((first, second) =>
            {
                second.PreviousNode.ConnectedNode = first;
                return second;
            });
        }
    }
}
