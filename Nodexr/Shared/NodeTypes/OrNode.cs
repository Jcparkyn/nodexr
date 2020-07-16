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
        public InputCheckbox InputCapture { get; } = new InputCheckbox(false)
        {
            Title = "Capture",
            DescriptionFunc = () => "Store the result using a capturing group."
        };

        [NodeInput]
        public InputCollection Inputs { get; } = new InputCollection("Option");

        public OrNode()
        {
            Inputs.AddItem();
            Inputs.AddItem();
        }

        /// <summary>
        /// Creates an OrNode with the given nodes as inputs.
        /// </summary>
        public OrNode(IEnumerable<INodeOutput> inputs)
        {
            foreach (var input in inputs)
            {
                Inputs.AddItem(input);
            }
        }

        /// <summary>
        /// Creates an OrNode with the given nodes as inputs.
        /// This overload is to allow easy instantiation with params.
        /// </summary>
        public OrNode(params INodeOutput[] inputs) : this(inputs as IEnumerable<INodeOutput>) { }

        protected override NodeResultBuilder GetValue()
        {
            var builder = new NodeResultBuilder();
            var inputs = Inputs.Inputs;
            string prefix = InputCapture.IsChecked ? "(" : "(?:";
            builder.Append(prefix, this);

            if (inputs.Count > 0)
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
