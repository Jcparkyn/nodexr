using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pidgin;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Nodexr.Shared.RegexParsers.ParsersShared;

namespace Nodexr.Shared.RegexParsers
{
    public static class LookaroundParser
    {
        public static Parser<char, LookaroundNode> ParseLookaround =>
            ParseLookaroundPrefix
            .Then(Rec(() => RegexParser.ParseRegex),
                (lookaround, contents) => lookaround.WithContents(contents));

        private static Parser<char, LookaroundNode> ParseLookaroundPrefix =>
            OneOf(
                ParseLookaheadPositive,
                ParseLookaheadNegative,
                ParseLookbehind);

        private static Parser<char, LookaroundNode> ParseLookaheadPositive =>
            Char('=').Select(_ => CreateWithType(LookaroundNode.Types.lookahead));

        private static Parser<char, LookaroundNode> ParseLookaheadNegative =>
            Char('!').Select(_ => CreateWithType(LookaroundNode.Types.lookaheadNeg));

        private static Parser<char, LookaroundNode> ParseLookbehind =>
            Try(
                Char('<')
                .Then(
                    Char('=')
                        .Select(_ => CreateWithType(LookaroundNode.Types.lookbehind))
                    .Or(Char('!')
                        .Select(_ => CreateWithType(LookaroundNode.Types.lookbehindNeg)))
                    ));

        private static LookaroundNode CreateWithType(LookaroundNode.Types type)
        {
            var node = new LookaroundNode();
            node.InputGroupType.Value = type;
            return node;
        }

        private static LookaroundNode WithContents(this LookaroundNode node, RegexNodeViewModelBase contents)
        {
            node.Input.ConnectedNode = contents;
            return node;
        }
    }
}
