using System.Collections.Generic;
using System.Net.Http.Headers;

namespace RegexNodes.Shared.NodeTypes
{
    public class IfElseNode : Node
    {
        public override string Title => "If-Else";
        public override string NodeInfo => "Matches either of two expressions, depending on whether the 'Condition' expression has matched. " +
            "\nIf the name or number of a captured group is used as the 'Condition' expression, it will be considered to have matched if the group it references was matched.";


        [NodeInput]
        public InputString InputCondition { get; } = new InputString("") { Title = "Condition:" };

        [NodeInput]
        public InputProcedural InputThen { get; set; } = new InputProcedural() { Title = "Match if true" };

        [NodeInput]
        public InputProcedural InputElse { get; set; } = new InputProcedural() { Title = "Match if false" };


        protected override NodeResultBuilder GetValue()
        {
            //return null;
            var builder = new NodeResultBuilder();
            //string condition = ConditionType.DropdownValue == "Expression" ? InputCondition.GetValue() : InputGroupID.GetValue();
            string condition = InputCondition.GetValue();

            builder.Append($"(?({condition})", this);
            builder.Append(InputThen.Value);
            builder.Append("|", this);
            builder.Append(InputElse.Value);
            builder.Append(")", this);
            //return $"(?({condition}){InputThen.GetValue()}|{InputElse.GetValue()})";
            return builder;
        }
    }
}
