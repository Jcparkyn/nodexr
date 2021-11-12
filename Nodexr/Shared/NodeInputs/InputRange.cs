namespace Nodexr.Shared.NodeInputs;
using BlazorNodes.Core;

public class InputRange : NodeInputBase
{
    private int? min;

    public int? Min
    {
        get => min;
        set
        {
            min = value;
            OnValueChanged();
        }
    }

    private int? max;

    public int? Max
    {
        get => max;
        set
        {
            if (AutoClearMax && value <= MinValue)
                max = null;
            else
                max = value;
            OnValueChanged();
        }
    }

    public int? MinValue { get; set; } = null;

    /// <summary>
    /// Automatically clear the 'Max' field if the user set the value below MinValue.
    /// </summary>
    public bool AutoClearMax { get; set; } = false;

    public override int Height => 50;

    public InputRange(int? min = null, int? max = null)
    {
        Min = min;
        Max = max;
    }
}
