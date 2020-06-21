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
    public class WildcardParser
    {
        public static Parser<char, WildcardNode> ParseWildcardAfterEscape =>
            OneOf(
                Digits,
                WordChars);

        public static Parser<char, WildcardNode> ParseWildcard =>
            Char('.')
            .WithResult(CreateWithInputs(a: true));

        private static Parser<char, WildcardNode> Digits =>
            UpperOrLower('d')
            .Select(isUpper => CreateWithInputs(invert: isUpper, d: true));
        
        private static Parser<char, WildcardNode> WordChars =>
            UpperOrLower('w')
            .Select(isUpper => CreateWithInputs(
                invert: isUpper,
                L: true,
                l: true,
                d: true,
                u: true
                ));

        private static WildcardNode CreateWithInputs(
            bool invert = false,
            bool a = false,
            bool w = false,
            bool L = false,
            bool l = false,
            bool d = false,
            bool u = false,
            bool o = false)
        {
            var node = new WildcardNode();

            node.InputAllowAll.IsChecked = a;
            node.InputAllowWhitespace.IsChecked = w ^ invert;
            node.InputAllowUppercase.IsChecked = L ^ invert;
            node.InputAllowLowercase.IsChecked = l ^ invert;
            node.InputAllowDigits.IsChecked = d ^ invert;
            node.InputAllowUnderscore.IsChecked = u ^ invert;
            node.InputAllowOther.IsChecked = o ^ invert;

            return node;
        }
    }
}
