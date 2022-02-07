namespace Nodexr.NodeInputs;

using BlazorNodes.Core;

public class InputString : NodeInputBase<string>
{
    private string _value;

    public override string Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged();
        }
    }

    public override int Height => 50;

    public InputString(string contents)
    {
        _value = contents;
    }

    public string GetValue() => Value;
}
