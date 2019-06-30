using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class OutputNode : Node
    {
        public override string Title => "Output";
        public override string NodeInfo => "The final output of your Regex. Use the 'Add Item' button to join together the outputs of multiple nodes, similar to the 'Concatenate' node.";

        [NodeInput]
        protected InputCollection Inputs { get; } = new InputCollection(1);

        public override string GetValue()
        {
            string result = "";
            foreach (var input in Inputs.Inputs)
            {
                result += input.GetValue();
            }
            CachedValue = result;
            return result;
        }
    }
}
