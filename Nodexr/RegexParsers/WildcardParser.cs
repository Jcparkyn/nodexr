using Pidgin;
using Nodexr.NodeTypes;
using static Pidgin.Parser;
using static Nodexr.RegexParsers.ParsersShared;

namespace Nodexr.RegexParsers
{
    public static class WildcardParser
    {
        public static Parser<char, WildcardNode> ParseWildcardAfterEscape =>
            OneOf(
                Digits,
                WordChars);

        public static Parser<char, WildcardNode> ParseWildcard =>
            Char('.')
            .Select(_ => CreateWithType(WildcardNode.WildcardType.Everything));

        private static Parser<char, WildcardNode> Digits =>
            UpperOrLower('d')
            .Select(isUpper => CreateWithType(WildcardNode.WildcardType.Digits, isUpper));

        private static Parser<char, WildcardNode> WordChars =>
            UpperOrLower('w')
            .Select(isUpper => CreateWithType(WildcardNode.WildcardType.WordCharacters, isUpper));

        private static WildcardNode CreateWithType(WildcardNode.WildcardType type, bool invert = false)
        {
            var node = new WildcardNode();
            node.InputType.Value = type;
            node.InputInvert.Checked = invert;
            return node;
        }
    }
}
