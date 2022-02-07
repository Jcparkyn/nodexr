namespace Nodexr.NodeInputs;
using BlazorNodes.Core;

public class InputNumber : NodeInputBase<int?>
{
    private int? _value;

    public override int? Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged();
        }
    }

    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }

    public override int Height => 50;

    public InputNumber(int contents, int? min = null, int? max = null)
    {
        Value = contents;
        MinValue = min;
        MaxValue = max;
    }
}
