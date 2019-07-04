using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class IfElseNode : Node
    {
        public override string Title => "If-Else";
        public override string NodeInfo => "Matches either of two expressions, depending on whether the 'if' expression has matched. " +
            "\nIf the name or number of a captured group is used as the 'if' expression, it will be considered to have matched if the group it references was matched.";


        //[NodeInput]
        //protected InputDropdown ConditionType { get; } = new InputDropdown(
        //    "Expression",
        //    "Captured Group")
        //{ Title = "Type:" };

        //[NodeInput]
        //protected InputProcedural InputCondition { get; set; } = new InputProcedural() { Title = "Condition" };

        [NodeInput]
        protected InputString InputGroupID { get; } = new InputString("") { Title = "Condition:" };

        [NodeInput]
        protected InputProcedural InputThen { get; set; } = new InputProcedural() { Title = "Match if true" };

        [NodeInput]
        protected InputProcedural InputElse { get; set; } = new InputProcedural() { Title = "Match if false" };

        //public IfElseNode()
        //{
        //    InputGroupID.IsEnabled = () => ConditionType.DropdownValue == "Captured Group";
        //    InputCondition.IsEnabled = () => ConditionType.DropdownValue == "Expression";
        //    ConditionType.OnValueChanged = CalculateInputsPos;
        //}

        public override string GetValue()
        {
            //string condition = ConditionType.DropdownValue == "Expression" ? InputCondition.GetValue() : InputGroupID.GetValue();
            string condition = InputGroupID.GetValue();
            
            return UpdateCache($"(?({condition}){InputThen.GetValue()}|{InputElse.GetValue()})");
        }
    }
}
