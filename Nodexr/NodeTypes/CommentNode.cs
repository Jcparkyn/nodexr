﻿using Pidgin;
using System.Collections.Generic;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;

namespace Nodexr.NodeTypes
{
    public class CommentNode : RegexNodeViewModelBase
    {
        public override string Title => "Comment";
        public override string NodeInfo => "Inserts a comment block, which will be ignored by the Regex engine.";

        [NodeInput]
        protected InputString Input { get; } = new InputString("");

        public CommentNode() { }
        public CommentNode(string contents)
        {
            Input.Value = contents;
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
