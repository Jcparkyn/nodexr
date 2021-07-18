using Pidgin;
using Nodexr.NodeTypes;
using Nodexr.Shared.Nodes;
using static Pidgin.Parser;
using static Nodexr.RegexParsers.ParsersShared;

namespace Nodexr.RegexParsers
{
    public static class IfElseParser
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
                .Slice((span, _) => span.ToString())
            .Between(OpenPar, ClosePar);

        //private static Parser<char, string> ParseIfElseCondition =>
        //    AnyCharExcept(')').AtLeastOnceString()
        //    .Between(OpenPar, ClosePar);

        private static Parser<char, RegexNodeViewModelBase> ParseIfElseOption =>
            Rec(() => RegexParser.ParseRegexWithoutAlternation);

        private static IfElseNode CreateIfElse(string condition, RegexNodeViewModelBase in1, RegexNodeViewModelBase in2)
        {
            var node = new IfElseNode();
            node.InputCondition.Value = condition;
            node.InputThen.ConnectedNode = in1;
            node.InputElse.ConnectedNode = in2;
            return node;
        }
    }
}
