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
    public class ReferenceParser
    {
        public static Parser<char, ReferenceNode> ParseReference =>
            IndexReference
            .Or(NamedReference);

        private static Parser<char, ReferenceNode> IndexReference =>
            Digit.Select(digit => CreateWithIndex(digit));

        private static Parser<char, ReferenceNode> NamedReference =>
            String("k<")
            .Then(
                Token(c => c != '>')
                .ManyString())
            .Before(Char('>'))
            .Select(name => CreateWithName(name));

        private static ReferenceNode CreateWithIndex(char digit)
        {
            int index = digit - '0';
            if (index > 9 || index < 0) throw new ArgumentOutOfRangeException(nameof(digit));
            var node = new ReferenceNode();
            node.InputType.Value = ReferenceNode.InputTypes.Index;
            node.InputIndex.InputContents = index;
            return node;
        }

        private static ReferenceNode CreateWithName(string name)
        {
            var node = new ReferenceNode();
            node.InputType.Value = ReferenceNode.InputTypes.Name;
            node.InputName.Contents = name;
            return node;
        }
    }
}
