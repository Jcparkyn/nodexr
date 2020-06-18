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
    public static class CharSetParser
    {
        private static readonly Parser<char, char> LBracket = Char('[');
        private static readonly Parser<char, char> RBracket = Char(']');

        private static readonly Parser<char, string> ValidCharSetChar =
            AnyCharExcept('\\', ']').Select(c => c.ToString())
            .Or(EscapeChar
                .Then(Any)
                .Select(c => "\\" + c)
                );

        //TODO: support negated sets
        private static readonly Parser<char, string> CharSetContents =
            ValidCharSetChar
            .AtLeastOnceString();

        public static readonly Parser<char, CharSetNode> ParseCharSet =
            LBracket.Then(CharSetContents).Before(RBracket)
                .Select(contents =>
                    new CharSetNode(contents));
    }
}
