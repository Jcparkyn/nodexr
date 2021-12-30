namespace Nodexr.RegexParsers;
using System.Collections.Generic;
using System.Linq;
using Pidgin;
using Nodexr.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Nodexr.RegexParsers.ParsersShared;
using Nodexr.Nodes;

public static class TextParser
{
    public static Parser<char, RegexNodeViewModelBase> ParseTextWithOptionalQuantifier =>
        Map(CreateTextWithQuantifier,
            ParseText,
            QuantifierParser.ParseQuantifier.Optional());

    public static Parser<char, IEnumerable<string>> ParseText =>
        NonSpecialRegexChar.AtLeastOnce();

    private static readonly Parser<char, string> NonSpecialRegexChar =
        AnyCharExcept("\\|?*+()[{.^$")
            .Select(c => c.ToString())
        .Or(Try(
            EscapeChar
            .Then(
                Not(RegexParser.ParseSpecialAfterEscape)
                .Then(Any))
            .Select(c => "\\" + c)
            ));

    public static RegexNodeViewModelBase CreateTextWithQuantifier(IEnumerable<string> chars, Maybe<QuantifierNode> maybeQuant)
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
