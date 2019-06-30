using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class CommentNode : Node
    {
        public override string Title => "Comment";
        public override string NodeInfo => "Inserts a comment block, which will be ignored by the Regex engine.";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { Input };
            }
        }

        protected InputString Input { get; set; } = new InputString("");

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
