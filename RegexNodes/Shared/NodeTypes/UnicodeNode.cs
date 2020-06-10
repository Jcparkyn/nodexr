using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class UnicodeNode : Node
    {
        public override string Title => "Unicode";

        public override string NodeInfo => "Insert a unicode category.";

        [NodeInput]
        protected InputString InputCategory { get; } = new InputString("IsBasicLatin") { Title = "Unicode Category" };
        [NodeInput]
        protected InputCheckbox InputInvert { get; } = new InputCheckbox() { Title = "Invert" };

        protected override string GetValue()
        {
            if (InputInvert.IsChecked)
            {
                return UpdateCache(@"\P{" + InputCategory.InputContents + "}");
            }
            else
            {
                return UpdateCache(@"\p{" + InputCategory.InputContents + "}");
            }
        }
    }
}
