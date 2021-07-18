using Pidgin;
using Nodexr.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Nodexr.RegexParsers.ParsersShared;

namespace Nodexr.RegexParsers
{
    public static class CharSetParser
    {
        private static readonly Parser<char, char> LBracket = Char('[');
        private static readonly Parser<char, char> RBracket = Char(']');
        private static readonly Parser<char, char> Hat = Char('^');

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
            Map((invert, contents) => CreateWithContents(contents, invert),
                Hat.WasMatched(),
                CharSetContents)
            .Between(LBracket, RBracket);

        private static CharSetNode CreateWithContents(string contents, bool invert)
        {
            var node = new CharSetNode();
            node.InputCharacters.Value = contents;
            node.InputDoInvert.Checked = invert;
            return node;
        }
    }
}
