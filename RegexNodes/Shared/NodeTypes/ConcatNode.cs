using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class ConcatNode : Node, INode
    {
        public override string Title => "Concatenate";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { Inputs };
            }
        }

        protected InputCollection Inputs { get; set; } = new InputCollection() { Title = "Inputs" };

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
