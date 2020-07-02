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
    public class OutputParser
    {
        public static Parser<char, OutputNode> ParseOutputNode =>
            OneOf(
                StartsAtWordBoundary,
                StartsAtStartOfLine,
                StartsAnywhere
                )
            .Then(RegexParser.ParseRegex,
                (output, main) => AttachOutputToContents(output, main)
                );

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
            if (contents is AnchorNode anchor)
            {
                switch (anchor.InputAnchorType.Value)
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
                    default:
                        output.Previous.ConnectedNode = contents;
                        break;
                }
            }
            else
            {
                output.Previous.ConnectedNode = contents;
            }

            return output;
        }
    }
}
