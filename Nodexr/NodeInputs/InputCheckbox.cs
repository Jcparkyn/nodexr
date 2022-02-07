namespace Nodexr.NodeInputs;
using BlazorNodes.Core;

public class InputCheckbox : NodeInputBase<bool>
{
    // TODO replace Checked
    public override bool Value { get => Checked; set => Checked = value; }

    private bool _checked;
    public bool Checked
    {
        get => _checked;
        set
        {
            _checked = value;
            OnValueChanged();
        }
    }

    public override int Height => 19;

    public InputCheckbox(bool isChecked = false)
    {
        _checked = isChecked;
    }
}
