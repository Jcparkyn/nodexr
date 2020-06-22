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
    public static class QuantifierParser
    {
        public static Parser<char, Node> WithOptionalQuantifier(this Parser<char, Node> previous) =>
            Map((prev, maybeQuant) =>
                    maybeQuant.HasValue ?
                        maybeQuant.Value.AttachToNode(prev) :
                        prev,
                previous,
                ParseQuantifier.Optional());

        //TODO: support lazy & possessive quantifiers
        public static Parser<char, QuantifierNode> ParseQuantifier =>
            OneOf(
                OneOrMore,
                ZeroOrMore,
                ZeroOrOne,
                Try(Number),
                Range)
            .Then(QuantifierSuffix,
                (node, searchType) => node.WithSearchType(searchType));

        private static Parser<char, string> QuantifierSuffix =>
            OneOf(
                Char('?').WithResult(QuantifierNode.SearchModes.lazy),
                Char('+').WithResult(QuantifierNode.SearchModes.possessive),
                Return(QuantifierNode.SearchModes.greedy));

        private static Parser<char, QuantifierNode> OneOrMore =>
            Char('+')
            .Select(_ => CreateWithRepetitions(QuantifierNode.Repetitions.oneOrMore)
            );

        private static Parser<char, QuantifierNode> ZeroOrMore =>
            Char('*')
            .Select(_ => CreateWithRepetitions(QuantifierNode.Repetitions.zeroOrMore)
            );

        private static Parser<char, QuantifierNode> ZeroOrOne =>
            Char('?')
            .Select(_ => CreateWithRepetitions(QuantifierNode.Repetitions.zeroOrOne));

        private static Parser<char, QuantifierNode> Number =>
            UnsignedInt(10)
            .Between(
                Char('{'),
                Char('}'))
            .Select(num => CreateWithNumber(num));

        private static Parser<char, QuantifierNode> Range =>
            UnsignedInt(10).Before(Char(','))
            .Then(OptionalInt, (num1, num2) => (min: num1, max: num2))
            .Between(
                Char('{'),
                Char('}'))
            .Select(range => CreateWithRange(range.min, range.max));

        private static readonly Parser<char, int?> OptionalInt =
            UnsignedInt(10)
            .OptionalOrNull();


        private static QuantifierNode CreateWithNumber(int number)
        {
            var node = new QuantifierNode();
            node.InputCount.DropdownValue = QuantifierNode.Repetitions.number;
            node.InputNumber.InputContents = number;
            return node;
        }

        private static QuantifierNode CreateWithRange(int min, int? max)
        {
            var node = new QuantifierNode();
            node.InputCount.DropdownValue = QuantifierNode.Repetitions.range;
            node.InputMin.InputContents = min;
            node.InputMax.InputContents = max;
            return node;
        }

        private static QuantifierNode CreateWithRepetitions(string repetitions)
        {
            var node = new QuantifierNode();
            node.InputCount.DropdownValue = repetitions;
            return node;
        }

        private static QuantifierNode WithSearchType(this QuantifierNode node, string searchType)
        {
            node.InputSearchType.DropdownValue = searchType;
            return node;
        }

        private static Node AttachToNode(this QuantifierNode quant, Node contents)
        {
            switch (contents)
            {
                case CharSetNode node when quant.InputSearchType.DropdownValue == QuantifierNode.SearchModes.greedy:
                    node.InputCount.DropdownValue = quant.InputCount.DropdownValue;
                    node.InputMin.InputContents = quant.InputMin.InputContents;
                    node.InputMax.InputContents = quant.InputMax.InputContents;
                    node.InputNumber.InputContents = quant.InputNumber.InputContents;
                    return node;
                case WildcardNode node when quant.InputSearchType.DropdownValue == QuantifierNode.SearchModes.greedy:
                    node.InputCount.DropdownValue = quant.InputCount.DropdownValue;
                    node.InputMin.InputContents = quant.InputMin.InputContents;
                    node.InputMax.InputContents = quant.InputMax.InputContents;
                    node.InputNumber.InputContents = quant.InputNumber.InputContents;
                    return node;
                default:
                    quant.InputContents.ConnectedNode = contents;
                    return quant;
            }
        }
    }
}
