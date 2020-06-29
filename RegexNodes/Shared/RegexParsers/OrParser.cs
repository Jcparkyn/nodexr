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
    public static class OrParser
    {
        public static Parser<char, Node> WithOptionalAlternation(this Parser<char, Node> previous) =>
            previous.SeparatedAtLeastOnce(Pipe)
            .Select(CreateWithInputs);

        private static readonly Parser<char, char> Pipe =
            Char('|');

        private static Node CreateWithInputs(IEnumerable<Node> nodes)
        {
            var nodesList = nodes.ToList();
            if(nodesList.Count == 1)
            {
                return nodesList.First();
            }

            var node = new OrNode();
            node.Inputs.RemoveAll();

            for (int i = 0; i < nodesList.Count; i++)
            {
                node.Inputs.AddItem();
                node.Inputs.Inputs.Last().ConnectedNode = nodesList[i];
            }

            return node;
        }
    }
}
