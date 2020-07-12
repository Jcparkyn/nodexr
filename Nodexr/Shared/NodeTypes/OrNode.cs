using System;
using System.Collections.Generic;
using System.Linq;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;

namespace Nodexr.Shared.NodeTypes
{
    public class OrNode : Node, INode
    {
        public override string Title => "Or";
        public override string NodeInfo => "Outputs a Regex that will accept any of the given inputs.";

        [NodeInput]
        public InputCollection Inputs { get; } = new InputCollection("Option", 2);

        protected override NodeResultBuilder GetValue()
        {
            var builder = new NodeResultBuilder();
            var inputs = Inputs.Inputs.Where(input => input.IsConnected);
            builder.Append(@"(?:", this);

            if (inputs.Any())
            {
                builder.Append(inputs.First().Value);
                foreach (var input in inputs.Skip(1))
                {
                    builder.Append("|", this);
                    builder.Append(input.Value);
                } 
            }
            builder.Append(")", this);

            return builder;
        }
    }
}
