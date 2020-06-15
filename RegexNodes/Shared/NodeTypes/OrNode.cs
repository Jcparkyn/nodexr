using System;
using System.Collections.Generic;
using System.Linq;

namespace RegexNodes.Shared.NodeTypes
{
    public class OrNode : Node, INode
    {
        public override string Title => "Or";
        public override string NodeInfo => "Outputs a Regex that will accept any of the given inputs.";

        [NodeInput]
        protected InputCollection Inputs { get; } = new InputCollection("Option", 2);

        protected override string GetValue()
        {
            //string result = $"(?:{Input1.GetValue()}|{Input2.GetValue()})";
            string result = @"(?:";

            result += String.Join("|", Inputs.Inputs.Select(input => input.GetValue()));
            result += ")";

            return result;
        }
    }
}
