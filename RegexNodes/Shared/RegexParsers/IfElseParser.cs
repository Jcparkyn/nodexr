using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pidgin;
using RegexNodes.Shared.NodeTypes;
using RegexNodes.Shared.Nodes;
using RegexNodes.Shared.NodeInputs;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static RegexNodes.Shared.RegexParsers.ParsersShared;

namespace RegexNodes.Shared.RegexParsers
{
    public class IfElseParser
    {
        public static Parser<char, IfElseNode> ParseIfElse =>
            Map((cond, in1, _, in2) => CreateIfElse(cond, in1, in2),
                ParseIfElseCondition,
                ParseIfElseOption,
                Char('|'),
                ParseIfElseOption
                );

        private static Parser<char, string> ParseIfElseCondition =>
            Rec(() => RegexParser.ParseRegex)
                .Slice((span, param) => span.ToString())
            .Between(OpenPar, ClosePar);

        //private static Parser<char, string> ParseIfElseCondition =>
        //    AnyCharExcept(')').AtLeastOnceString()
        //    .Between(OpenPar, ClosePar);

        private static Parser<char, Node> ParseIfElseOption =>
            Rec(() => RegexParser.ParseRegexWithoutAlternation);

        private static IfElseNode CreateIfElse(string condition, Node in1, Node in2)
        {
            var node = new IfElseNode();
            node.InputCondition.Contents = condition;
            node.InputThen.ConnectedNode = in1;
            node.InputElse.ConnectedNode = in2;
            return node;
        }
    }
}
