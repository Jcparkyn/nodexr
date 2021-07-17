using System.Collections.Generic;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using BlazorNodes.Core;

namespace Nodexr.NodeTypes
{
    public class LookaroundNode : RegexNodeViewModelBase
    {
        public override string Title => "Lookaround";
        public override string NodeInfo => "Converts the input node into a lookahead or lookbehind.";

        [NodeInput]
        public InputProcedural Input { get; } = new InputProcedural() { Title = "Contents" };
        [NodeInput]
        public InputDropdown<Types> InputGroupType { get; } = new InputDropdown<Types>(groupTypeDisplyNames)
        { Title = "Type:" };

        public enum Types
        {
            lookahead,
            lookbehind,
            lookaheadNeg,
            lookbehindNeg,
        }

        private static readonly Dictionary<Types, string> groupTypeDisplyNames = new()
        {
            { Types.lookahead, "Lookahead" },
            { Types.lookbehind, "Lookbehind" },
            { Types.lookaheadNeg, "Negative Lookahead" },
            { Types.lookbehindNeg, "Negative Lookbehind" },
        };

        protected override NodeResultBuilder GetValue()
        {
            var builder = new NodeResultBuilder(Input.Value);

            if (Input.ConnectedNode is OrNode)
                builder.StripNonCaptureGroup();

            string prefix = InputGroupType.Value switch
            {
                Types.lookahead => "(?=",
                Types.lookbehind => "(?<=",
                Types.lookaheadNeg => "(?!",
                Types.lookbehindNeg => "(?<!",
                _ => "",
            };
            builder.Prepend(prefix, this);
            builder.Append(")", this);
            return builder;
        }
    }
}
