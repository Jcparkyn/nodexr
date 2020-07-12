using Pidgin;
using System.Collections.Generic;
using RegexNodes.Shared.Nodes;
using RegexNodes.Shared.NodeInputs;

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

        protected override NodeResultBuilder GetValue()
        {
            string result = "(?#" + Input.GetValue()
                //.Replace("(", "")
                .Replace(")", "")
                + ")";

            var builder = new NodeResultBuilder(result, this);
            return builder;
        }
    }
}
