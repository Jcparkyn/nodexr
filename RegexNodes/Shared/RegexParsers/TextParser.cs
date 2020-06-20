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
    public class TextParser
    {
        public static Parser<char, Node> ParseTextWithOptionalQuantifier =>
            Map(CreateTextWithQuantifier,
                NonSpecialRegexChar.AtLeastOnce(),
                QuantifierParser.ParseQuantifier.Optional());

        private static readonly Parser<char, string> NonSpecialRegexChar =
            AnyCharExcept("\\|?*+()[{")
                .Select(c => c.ToString())
            .Or(EscapeChar
                .Then(Any)
                .Select(c => "\\" + c)
                );

        public static Node CreateTextWithQuantifier(IEnumerable<string> chars, Maybe<QuantifierNode> maybeQuant)
        {
            if (!maybeQuant.HasValue)
                return TextNode.CreateWithContents(string.Concat(chars));

            var quant = maybeQuant.Value;
            if (chars.Count() == 1)
            {
                var textNode = TextNode.CreateWithContents(chars.First());
                quant.InputContents.ConnectedNode = textNode;
                return quant;
            }

            var mainText = TextNode.CreateWithContents(string.Concat(chars.SkipLast(1)));
            var lastChar = TextNode.CreateWithContents(chars.Last());

            quant.PreviousNode = mainText;
            quant.InputContents.ConnectedNode = lastChar;

            return quant;
        }
    }
}
