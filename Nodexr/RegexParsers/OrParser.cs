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
using static Nodexr.RegexParsers.ParsersShared;

namespace Nodexr.RegexParsers
{
    public static class OrParser
    {
        public static Parser<char, RegexNodeViewModelBase> WithOptionalAlternation(this Parser<char, RegexNodeViewModelBase> previous) =>
            previous.SeparatedAtLeastOnce(Pipe)
            .Select(CreateWithInputs);

        private static readonly Parser<char, char> Pipe =
            Char('|');

        private static RegexNodeViewModelBase CreateWithInputs(IEnumerable<RegexNodeViewModelBase> nodes)
        {
            var nodesList = nodes.ToList();
            if(nodesList.Count == 1)
            {
                return nodesList[0];
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
