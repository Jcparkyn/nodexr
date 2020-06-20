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
        //public static Parser<char, QuantifierNode> ParseQuantifierOptional(this Parser<char, Node> previous) =>
        //    Map((Node node, QuantifierNode quant) => quant.WithContents(node),
        //        previous,
        //        QuantifierPrefix)
        //    .Optional()
        //    .Select(maybe => maybe.HasValue ?
        //        maybe.Value :
        //        previous.);

        public static Parser<char, Node> WithOptionalQuantifier(this Parser<char, Node> previous) =>
            Map((prev, maybeQuant) =>
                    maybeQuant.HasValue ?
                        maybeQuant.Value.AttachToNode(prev) :
                        prev,
                previous,
                ParseQuantifier.Optional());

        public static Parser<char, QuantifierNode> ParseQuantifier =>
            OneOf(
                OneOrMore,
                ZeroOrMore,
                ZeroOrOne,
                Try(Number),
                Range);

        //public static Parser<char, QuantifierNode> ParseQuantifier =>
        //    OneOrMore;

        private static Parser<char, QuantifierNode> OneOrMore =>
            Char('+')
            .WithResult(CreateWithRepetitions(QuantifierNode.Repetitions.oneOrMore)
            );

        private static Parser<char, QuantifierNode> ZeroOrMore =>
            Char('*')
            .WithResult(CreateWithRepetitions(QuantifierNode.Repetitions.zeroOrMore)
            );

        private static Parser<char, QuantifierNode> ZeroOrOne =>
            Char('?')
            .WithResult(CreateWithRepetitions(QuantifierNode.Repetitions.zeroOrOne)
            );

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

        //private static readonly Parser<char, QuantifierNode> NumberOrRange =
        //    UnsignedInt(10)
        //    .Separated(Char(','))
        //    .Between(
        //            Char('{'),
        //            Char('}'))
        //    .Select(numbers => numbers.Count() switch
        //    {
        //        1 => CreateWithNumber(numbers.First()),
        //        2 => Cre
        //    });

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

        private static QuantifierNode AttachToNode(this QuantifierNode quant, Node contents)
        {
            switch (contents)
            {
                //case TextNode node: //If contents is a TextNode, we only want to quantify the last char of text
                //    quant.QuantifyLastCharOfTextNode(node); break;
                default:
                    quant.InputContents.ConnectedNode = contents; break;
            }
            
            return quant;
        }

        //private static void QuantifyLastCharOfTextNode(this QuantifierNode quant, TextNode textNode)
        //{
        //    string text = textNode.Input.Contents;

        //    if (text.IsSingleRegexChar())
        //    {
        //        quant.InputContents.ConnectedNode = textNode;
        //        return;
        //    }

        //    str
        //}
    }
}
