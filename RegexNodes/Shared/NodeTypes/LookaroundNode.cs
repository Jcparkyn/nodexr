using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class LookaroundNode : Node
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

        private static readonly Dictionary<Types, string> groupTypeDisplyNames = new Dictionary<Types, string>()
        {
            {Types.lookahead, "Lookahead"},
            {Types.lookbehind, "Lookbehind"},
            {Types.lookaheadNeg, "Negative Lookahead"},
            {Types.lookbehindNeg, "Negative Lookbehind"},
        };

        protected override string GetValue()
        {
            string input = Input.GetValue().RemoveNonCapturingGroup();
            string prefix = "";
            switch (InputGroupType.Value)
            {
                case Types.lookahead: prefix = "(?="; break;
                case Types.lookbehind: prefix = "(?<="; break;
                case Types.lookaheadNeg: prefix = "(?!"; break;
                case Types.lookbehindNeg: prefix = "(?<!"; break;
            };
            
            return $"{prefix}{input})";
        }
    }
}
