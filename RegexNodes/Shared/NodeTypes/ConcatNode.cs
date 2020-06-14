using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class ConcatNode : Node, INode
    {
        public override string Title => "Concatenate";
        public override string NodeInfo => "Concatenates (strings together) the outputs of multiple nodes, so that they come one after another. This could also be thought of as an 'And' or 'Then' node. The resulting regex will be order-sensitive, so order your inputs from top to bottom.";

        [NodeInput]
        protected InputCollection Inputs { get; } = new InputCollection() { Title = "Inputs" };

        protected override string GetValue()
        {
            string result = "";
            foreach (var input in Inputs.Inputs)
            {
                result += input.GetValue();
            }
            return result;
        }
    }
}
