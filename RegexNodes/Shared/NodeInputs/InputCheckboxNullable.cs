using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeInputs
{
    public class InputCheckboxNullable : NodeInput
    {

        private int checkedState;

        public int CheckedState
        {
            get => checkedState;
            set
            {
                checkedState = value;
                OnValueChanged();
            }
        }

        public override int Height => 19;

        public InputCheckboxNullable(int state = 0)
        {
            this.checkedState = state;
        }
    }
}
