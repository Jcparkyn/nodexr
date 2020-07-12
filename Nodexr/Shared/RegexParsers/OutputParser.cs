using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pidgin;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Nodexr.Shared.RegexParsers.ParsersShared;

namespace Nodexr.Shared.RegexParsers
{
    public class OutputParser
    {
        public static Parser<char, OutputNode> ParseOutputNode =>
            RegexParser.ParseRegex
            .Select(contents => AttachOutputToContents(new OutputNode(), contents));
            //OneOf(
            //    StartsAtWordBoundary,
            //    StartsAtStartOfLine,
            //    StartsAnywhere
            //    )
            //.Then(RegexParser.ParseRegex,
            //    (output, main) => AttachOutputToContents(output, main)
            //    );

        private static Parser<char, OutputNode> StartsAtWordBoundary =>
            Try(String("\\b"))
            .Select(_ => CreateWithStartsAt(OutputNode.Mode.WordBound));

        private static Parser<char, OutputNode> StartsAtStartOfLine =>
            Char('^')
            .Select(_ => CreateWithStartsAt(OutputNode.Mode.StartLine));

        private static Parser<char, OutputNode> StartsAnywhere =>
            ReturnLazy(() => CreateWithStartsAt(OutputNode.Mode.Anywhere));

        private static OutputNode CreateWithStartsAt(OutputNode.Mode startsAt)
        {
            var node = new OutputNode();
            node.InputStartsAt.Value = startsAt;
            return node;
        }

        private static OutputNode AttachOutputToContents(OutputNode output, Node contents)
        {
            output.PreviousNode = contents;

            if (contents is AnchorNode anchorEnd)
            {
                switch (anchorEnd.InputAnchorType.Value)
                {
                    case AnchorNode.Mode.EndLine:
                        output.InputEndsAt.Value = OutputNode.Mode.EndLine;
                        output.PreviousNode = contents.PreviousNode;
                        contents.PreviousNode = null;
                        break;
                    case AnchorNode.Mode.WordBoundary:
                        output.InputEndsAt.Value = OutputNode.Mode.WordBound;
                        output.PreviousNode = contents.PreviousNode;
                        contents.PreviousNode = null;
                        break;
                }
            }

            var previousNodes = GetPreviousNodes(output).ToList();
            if(previousNodes.Last() is AnchorNode anchorStart)
            {
                var anchorParent = previousNodes[^2];
                switch (anchorStart.InputAnchorType.Value)
                {
                    case AnchorNode.Mode.StartLine:
                        output.InputStartsAt.Value = OutputNode.Mode.StartLine;
                        anchorParent.PreviousNode = null;
                        break;
                    case AnchorNode.Mode.WordBoundary:
                        output.InputStartsAt.Value = OutputNode.Mode.WordBound;
                        anchorParent.PreviousNode = null;
                        break;
                }
            }
            return output;
        }

        private static IEnumerable<Node> GetPreviousNodes(Node parent)
        {
            var currentNode = parent;
            while (currentNode.PreviousNode is Node previous)
            {
                yield return previous;
                currentNode = previous;
            }
        }

        private static Node GetEarliestPreviousNode(Node parent)
        {
            var currentNode = parent;
            while (currentNode.PreviousNode is Node previous)
            {
                currentNode = previous;
            }
            return currentNode;
        }
    }
}
