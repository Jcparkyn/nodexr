namespace Nodexr.NodeInputs;
using BlazorNodes.Core;

public class InputCheckboxNullable : NodeInputBase<int>
{
    // TODO replace CheckedState
    public override int Value { get => CheckedState; set => CheckedState = value; }

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
        checkedState = state;
    }
}
