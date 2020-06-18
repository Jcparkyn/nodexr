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

        public static readonly Parser<char, TextNode> ParseTextNode =
            NonSpecialRegexPhrase
            .Select(text => TextNode.CreateFromContents(text));

        

        public static Parser<char, Node> ParseRegex =>
            ParseSingleNode
            .AtLeastOnce()
            .Select(ConnectNodesInSequence);


        public static readonly Parser<char, Node> ParseSingleNode =
            CharSetParser.ParseCharSet.Cast<Node>()
            .Or(ParseTextNode.Cast<Node>())
            .Or(Rec(() => GroupParser.ParseGroup).Cast<Node>());


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
