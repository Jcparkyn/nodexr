using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class CommentNode : Node
    {
        public override string Title => "Comment";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { Input };
            }
        }

        public InputString Input { get; set; } = new InputString("");

        public CommentNode() { }
        public CommentNode(string contents)
        {
            Input.InputContents = contents;
        }

        public override string GetValue()
        {
            string result = "(?#" + Input.GetValue()
                .Replace("(", "")
                .Replace(")", "")
                + ")";

            return UpdateCache(result);
        }
    }
}
