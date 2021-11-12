namespace Nodexr.RegexParsers;
using System.Collections.Generic;
using System.Linq;
using Pidgin;
using Nodexr.Shared.Nodes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Nodexr.RegexParsers.ParsersShared;
using BlazorNodes.Core;

public static class RegexParser
{
    public static readonly Parser<char, RegexNodeViewModelBase?> ParseRegex =
        ParseRegexWithoutAlternation
        .Cast<RegexNodeViewModelBase?>()
        .Or(Return<RegexNodeViewModelBase?>(null)) //Empty OrNode options are allowed.
        .WithOptionalAlternation();

    public static Parser<char, RegexNodeViewModelBase> ParseRegexWithoutAlternation =>
        ParseSingleNode
        .AtLeastOnce()
        .Select(ConnectNodesInSequence);

    private static Parser<char, RegexNodeViewModelBase> ParseSingleNode =>
        TextParser.ParseTextWithOptionalQuantifier
        .Or(
            OneOf(
                CharSetParser.ParseCharSet.Cast<RegexNodeViewModelBase>(),
                GroupParser.ParseGroup.Cast<RegexNodeViewModelBase>(),
                WildcardParser.ParseWildcard.Cast<RegexNodeViewModelBase>(),
                AnchorParser.ParseAnchor.Cast<RegexNodeViewModelBase>(),
                ParseEscapedWord)
            .WithOptionalQuantifier());

    public static Parser<char, RegexNodeViewModelBase> ParseEscapedWord =>
        EscapeChar.Then(ParseSpecialAfterEscape);

    public static Parser<char, RegexNodeViewModelBase> ParseSpecialAfterEscape =>
        OneOf(
            UnicodeParser.ParseUnicode.Cast<RegexNodeViewModelBase>(),
            ReferenceParser.ParseReference.Cast<RegexNodeViewModelBase>(),
            WildcardParser.ParseWildcardAfterEscape.Cast<RegexNodeViewModelBase>(),
            AnchorParser.ParseAnchorAfterEscape.Cast<RegexNodeViewModelBase>(),
            WhitespaceParser.ParseWhitespaceAfterEscape.Cast<RegexNodeViewModelBase>());

    public static Result<char, NodeTree> Parse(string input)
    {
        return OutputParser.ParseOutputNode.Select(BuildNodeTree).Parse(input);
    }

    public static NodeTree BuildNodeTree(RegexNodeViewModelBase endNode)
    {
        var builder = new NodeTreeBuilder(endNode);
        return builder.Build();
    }

    private static RegexNodeViewModelBase ConnectNodesInSequence(IEnumerable<RegexNodeViewModelBase> nodes)
    {
        var endNode = nodes.Aggregate((first, second) =>
        {
            first.ConnectBefore(second);
            return second;
        });

        return endNode;
    }

    private static void ConnectBefore(this RegexNodeViewModelBase first, RegexNodeViewModelBase second)
    {
        if (second.PreviousNode is null)
        {
            second.PreviousNode = first;
            return;
        }
        else
        {
            first.ConnectBefore((RegexNodeViewModelBase)second.PreviousNode);
        }
    }
}
