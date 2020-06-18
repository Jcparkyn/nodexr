using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class CommentNode : Node
    {
        public override string Title => "Comment";
        public override string NodeInfo => "Inserts a comment block, which will be ignored by the Regex engine.";

        [NodeInput]
        protected InputString Input { get; } = new InputString("");

        public CommentNode() { }
        public CommentNode(string contents)
        {
            Input.Contents = contents;
        }

        protected override string GetValue()
        {
            string result = "(?#" + Input.GetValue()
                .Replace("(", "")
                .Replace(")", "")
                + ")";

            return result;
        }
    }
}
