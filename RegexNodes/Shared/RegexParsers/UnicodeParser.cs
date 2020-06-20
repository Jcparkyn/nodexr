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
    public class UnicodeParser
    {
        public static Parser<char, UnicodeNode> ParseUnicode => 
            OneOf(
                HexX,
                HexU,
                UnicodeClass);

        private static Parser<char, UnicodeNode> HexX =>
            Char('x')
                .Then(HexString(2))
                .Select(hex => CreateWithHexCode(hex));

        private static Parser<char, UnicodeNode> HexU =>
            Char('u')
                .Then(HexString(4))
                .Select(hex => CreateWithHexCode(hex));

        private static Parser<char, UnicodeNode> UnicodeClass =>
            Token(c => c != '}')
            .AtLeastOnceString()
            .Between(Char('{'), Char('}'))
            .Select(str => CreateWithCategory(str));


        private static Parser<char, string> HexString(int length) =>
            LetterOrDigit
            .RepeatString(length);

        private static UnicodeNode CreateWithCategory(string unicodeCategory)
        {
            var node = new UnicodeNode();
            node.InputMode.DropdownValue = UnicodeNode.Modes.category;
            node.InputCategory.Contents = unicodeCategory;
            return node;
        }

        private static UnicodeNode CreateWithHexCode(string hex)
        {
            var node = new UnicodeNode();
            node.InputMode.DropdownValue = UnicodeNode.Modes.hex;
            node.InputHexCode.Contents = hex;
            return node;
        }
    }
}
