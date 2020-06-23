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
                Char('^').Select(_ => CreateWithType(AnchorNode.Modes.startLine)),
                Char('$').Select(_ => CreateWithType(AnchorNode.Modes.endLine))
                );

        public static Parser<char, AnchorNode> ParseAnchorAfterEscape =>
            Char('b').Select(_ => CreateWithType(AnchorNode.Modes.wordBoundary))
            .Or(Char('B').Select(_ => CreateWithType(AnchorNode.Modes.notWordBoundary)));

        private static AnchorNode CreateWithType(string type)
        {
            var node = new AnchorNode();
            node.InputAnchorType.DropdownValue = type;
            return node;
        }
    }
}
