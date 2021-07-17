using Pidgin;
using Nodexr.NodeTypes;
using static Nodexr.RegexParsers.ParsersShared;

namespace Nodexr.RegexParsers
{
    public static class WhitespaceParser
    {
        public static Parser<char, WhitespaceNode> ParseWhitespaceAfterEscape =>
            UpperOrLower('s')
            .Select(isUpper => CreateWhitespaceNode(invert: isUpper));

        private static WhitespaceNode CreateWhitespaceNode(bool invert)
        {
            var node = new WhitespaceNode();
            node.InputInvert.Checked = invert;
            return node;
        }
    }
}
