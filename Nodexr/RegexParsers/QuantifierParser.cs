using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pidgin;
using Nodexr.NodeTypes;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Nodexr.RegexParsers.ParsersShared;
using static Nodexr.NodeTypes.IQuantifiableNode;

namespace Nodexr.RegexParsers
{
    public static class QuantifierParser
    {
        public static Parser<char, RegexNodeViewModelBase> WithOptionalQuantifier(this Parser<char, RegexNodeViewModelBase> previous) =>
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

        private static Parser<char, QuantifierNode.SearchMode> QuantifierSuffix =>
            OneOf(
                Char('?').WithResult(QuantifierNode.SearchMode.Lazy),
                Char('+').WithResult(QuantifierNode.SearchMode.Possessive),
                Return(QuantifierNode.SearchMode.Greedy));

        private static Parser<char, QuantifierNode> OneOrMore =>
            Char('+')
            .Select(_ => CreateWithRepetitions(Reps.OneOrMore)
            );

        private static Parser<char, QuantifierNode> ZeroOrMore =>
            Char('*')
            .Select(_ => CreateWithRepetitions(Reps.ZeroOrMore)
            );

        private static Parser<char, QuantifierNode> ZeroOrOne =>
            Char('?')
            .Select(_ => CreateWithRepetitions(Reps.ZeroOrOne));

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
            node.InputCount.Value = Reps.Number;
            node.InputNumber.Value = number;
            return node;
        }

        private static QuantifierNode CreateWithRange(int min, int? max)
        {
            var node = new QuantifierNode();
            node.InputCount.Value = Reps.Range;
            node.InputRange.Min = min;
            node.InputRange.Max = max;
            return node;
        }

        private static QuantifierNode CreateWithRepetitions(Reps repetitions)
        {
            var node = new QuantifierNode();
            node.InputCount.Value = repetitions;
            return node;
        }

        private static QuantifierNode WithSearchType(this QuantifierNode node, QuantifierNode.SearchMode searchType)
        {
            node.InputSearchType.Value = searchType;
            return node;
        }

        private static RegexNodeViewModelBase AttachToNode(this QuantifierNode quant, RegexNodeViewModelBase contents)
        {
            switch (contents)
            {
                case IQuantifiableNode child when
                contents.PreviousNode is null
                && quant.InputSearchType.Value == QuantifierNode.SearchMode.Greedy:
                    //Transfer properties to child node if Quantifiable
                    child.InputCount.Value = quant.InputCount.Value;
                    child.InputRange.Min = quant.InputRange.Min;
                    child.InputRange.Max = quant.InputRange.Max;
                    child.InputNumber.Value = quant.InputNumber.Value;
                    return child as RegexNodeViewModelBase;

                case GroupNode child when
                child.PreviousNode is null
                && child.InputGroupType.Value == GroupNode.GroupTypes.nonCapturing:
                    //Discard unneseccary non-capturing group
                    quant.InputContents.ConnectedNode = child.Input.ConnectedNode;
                    return quant;

                default:
                    quant.InputContents.ConnectedNode = contents;
                    return quant;
            }
        }
    }
}
