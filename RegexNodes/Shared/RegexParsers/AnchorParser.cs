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
    public class AnchorParser
    {
        public static Parser<char, AnchorNode> ParseAnchor =>
            OneOf(
                Char('^').Select(_ => CreateWithType(AnchorNode.Mode.StartLine)),
                Char('$').Select(_ => CreateWithType(AnchorNode.Mode.EndLine))
                );

        public static Parser<char, AnchorNode> ParseAnchorAfterEscape =>
            Char('b').Select(_ => CreateWithType(AnchorNode.Mode.WordBoundary))
            .Or(Char('B').Select(_ => CreateWithType(AnchorNode.Mode.NotWordBoundary)));

        private static AnchorNode CreateWithType(AnchorNode.Mode type)
        {
            var node = new AnchorNode();
            node.InputAnchorType.Value = type;
            return node;
        }
    }
}
