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
        public InputDropdown InputGroupType { get; } = new InputDropdown(
            Types.lookahead,
            Types.lookbehind,
            Types.lookaheadNeg,
            Types.lookbehindNeg)
        { Title = "Type:" };

        public class Types
        {
            public const string lookahead = "Lookahead";
            public const string lookbehind = "Lookbehind";
            public const string lookaheadNeg = "Negative Lookahead";
            public const string lookbehindNeg = "Negative Lookbehind";
        }

        protected override string GetValue()
        {
            string input = Input.GetValue().RemoveNonCapturingGroup();
            string prefix = "";
            switch (InputGroupType.DropdownValue)
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
