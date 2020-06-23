using System.Collections.Generic;

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

        //public IfElseNode()
        //{
        //    InputGroupID.IsEnabled = () => ConditionType.DropdownValue == "Captured Group";
        //    InputCondition.IsEnabled = () => ConditionType.DropdownValue == "Expression";
        //    ConditionType.OnValueChanged = CalculateInputsPos;
        //}

        protected override string GetValue()
        {
            //string condition = ConditionType.DropdownValue == "Expression" ? InputCondition.GetValue() : InputGroupID.GetValue();
            string condition = InputCondition.GetValue();
            
            return $"(?({condition}){InputThen.GetValue()}|{InputElse.GetValue()})";
        }
    }
}
